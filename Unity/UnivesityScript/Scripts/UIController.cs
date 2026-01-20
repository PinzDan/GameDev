using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum tipoMenu
{
    main,
    ingame,
}
public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    public UIDocument uIDocument;
    public GameObject coinsScript;
    public GameObject hotbarUI;

    public GameObject audioManager;
    private GameObject hoverAudio;
    private GameObject clickAudio;


    private Button play;
    private Button exit;
    private Button settings;

    private Button _continue;



    public tipoMenu tipo;
    public VisualTreeAsset mainMenu; // Drag del template MainMenu
    public VisualTreeAsset WithSavesMainMenu; // Drag del template InGameMenu
    public VisualTreeAsset Settings;

    public GameObject insertName;
    private bool nameConfirmed = false;

    //altri riferimenti
    [SerializeField] private StateManager stateManager; // Aggiunto per gestire gli stati del gioco

    //Movimenti di camera del menu
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private Camera menuCamera;
    [SerializeField] private UIDocument hotbarDocument;

    [SerializeField] private SettingsUI settingsUI;
    private VisualElement hotbarContainer;

    public VisualTreeAsset continueButtonUXML; // Drag del template ContinueButton
    private List<Button> continueButtons = new List<Button>();

    private List<VisualElement> hotbarSlots = new List<VisualElement>();
    public bool hasAnySave;
    private VisualTreeAsset previousVisualTreeAsset;
    private void OnEnable()
    {


        settingsUI.enabled = false;
        UIDocument uIDocument = GetComponent<UIDocument>();

        hoverAudio = audioManager.transform.Find("HoverAudio").gameObject;
        clickAudio = audioManager.transform.Find("ClickAudio").gameObject;
        if (uIDocument == null)
            return;

        if (tipo == tipoMenu.ingame)
        {
            Debug.Log("Tipo menu ingame");
            RegisterMenuButtonsInGame();
        }
        if (tipo == tipoMenu.main)
        {

            hasAnySave = false;
            for (int i = 1; i <= 3; i++)
            {
                if (SaveSystem.SlotEsiste(i))
                {
                    hasAnySave = true;
                    break;
                }
            }

            init();

            // Inizializza hotbar //

            if (hotbarDocument != null)
            {
                hotbarContainer = hotbarDocument.rootVisualElement.Q<VisualElement>("FlexContainer");

                foreach (var child in hotbarContainer.Children())
                {
                    hotbarSlots.Add(child);
                }

                Debug.Log($"Hotbar inizializzata con {hotbarSlots.Count} slot.");
            }
            else
            {
                Debug.LogWarning("Hotbar UIDocument non assegnato al UIController.");
            }


        }

    }


    private void Continua(int slot)
    {
        var dati = SaveSystem.Carica(slot);
        if (dati != null)
        {
            Debug.Log($"Continua dal salvataggio slot {slot}");
            // Logica per ripristinare lo stato del gioco
            Play(slot); // ad esempio chiude il menu e riprende il gioco
        }
    }




    private void Play(int slot = 1)
    {
        Debug.Log("Play button clicked slot" + slot);
        SaveSystem.NuovoGioco(slot);
        GameManager.Instance.CaricaGioco(slot);


        var dati = SaveSystem.Carica(slot);

        uIDocument.rootVisualElement.style.display = DisplayStyle.None;
        if (!string.IsNullOrEmpty(dati.playerName))
        {
            avviaGame();
            return;
        }
        else /* se non ha nome: */
        {
            insertName.SetActive(true);
            StartCoroutine(WaitForNameConfirmation());
        }

    }

    private void resumeGame()
    {
        uIDocument.rootVisualElement.style.display = DisplayStyle.None;
        avviaGame();
    }

    private IEnumerator WaitForNameConfirmation()
    {
        nameConfirmed = false;
        while (!nameConfirmed)
            yield return null;

        avviaGame();

    }

    private void avviaGame()
    {
        coinsScript.SetActive(true);
        hotbarUI.SetActive(true);
        Debug.Log("Avvio game");
        stateManager.Resume();
        if (freeLookCamera != null)
        {
            Debug.Log("PRemuto");
            freeLookCamera.enabled = true;
        }
        if (menuCamera != null)
        {
            menuCamera.gameObject.SetActive(false);
        }
    }

    private void OnButtonHoverEnter(Button btn)
    {
        hoverAudio.GetComponent<AudioSource>().Play();
    }

    private void OnButtonHoverExit(Button btn)
    {
        hoverAudio.GetComponent<AudioSource>().Play();
    }

    private void OnButtonClick(Button settings)
    {
        Debug.Log("Button clicked: " + settings.name);
        clickAudio.GetComponent<AudioSource>().Play();

        switch (settings.name)
        {
            case "Play":
                Play();
                // Logica per avviare il gioco
                break;
            case "Exit":
                if (tipo == tipoMenu.ingame)
                    GameManager.Instance.SalvaGioco(GameManager.Instance.dati.slot);

                Application.Quit();
                break;
            case "Settings":


                previousVisualTreeAsset = uIDocument.visualTreeAsset;

                uIDocument.rootVisualElement.style.display = DisplayStyle.None;
                uIDocument.visualTreeAsset = Settings;
                uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
                settingsUI.enabled = true;

                // Cerca il bottone "Back" e collega l'evento
                registerBackButton();
                break;
            case "Continue":
                resumeGame();
                break;
            case "Back":
                uIDocument.rootVisualElement.style.display = DisplayStyle.None;
                Debug.Log("Previous VisualTreeAsset: " + previousVisualTreeAsset);
                uIDocument.visualTreeAsset = previousVisualTreeAsset;
                Debug.Log("Current VisualTreeAsset after Back: " + uIDocument.visualTreeAsset);
                uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;
                settingsUI.enabled = false;
                if (tipo == tipoMenu.main)
                {
                    init();

                }
                else
                    RegisterMenuButtonsInGame();


                break;
            default:
                Debug.LogWarning("Unknown button clicked: " + settings.name);
                break;
        }
    }


    public void OnNameConfirmed()
    {
        nameConfirmed = true;
        insertName.SetActive(false);


    }



    private void init()
    {
        if (hasAnySave)
        {
            uIDocument.visualTreeAsset = WithSavesMainMenu;
            // Dopo aver assegnato il visualTreeAsset, aggiorna i testi dei bottoni
            for (int i = 1; i <= 3; i++)
            {
                var button = uIDocument.rootVisualElement.Q<Button>($"Slot{i}Button");
                button.RegisterCallback<MouseEnterEvent>(evt => OnButtonHoverEnter(exit));
                button.RegisterCallback<MouseLeaveEvent>(evt => OnButtonHoverExit(exit));

                button.RegisterCallback<MouseEnterEvent>(evt => OnButtonHoverEnter(settings));
                button.RegisterCallback<MouseLeaveEvent>(evt => OnButtonHoverExit(settings));
                int slotIndex = i; // Cattura l'indice del ciclo
                if (button != null)
                {
                    if (SaveSystem.SlotEsiste(i))
                    {
                        var dati = SaveSystem.Carica(i);
                        Debug.Log($"Dati caricati dallo slot {i}: Nome giocatore - {dati.playerName}, Punteggio - {dati.punteggioTotale}");
                        button.text = $"{dati.playerName}:  Punteggio: {dati.punteggioTotale}";
                        button.RegisterCallback<ClickEvent>(evt => Continua(slotIndex));
                    }
                    else
                    {
                        button.text = $"Slot {i}:  Nuovo Gioco";
                        button.RegisterCallback<ClickEvent>(evt => Play(slotIndex));
                    }

                    Debug.Log($"Button Slot{i} text set to: {button.text}");

                }
            }

        }
        else
            uIDocument.visualTreeAsset = mainMenu;


        RegisterMenuButtonsMain();
        uIDocument.rootVisualElement.style.display = DisplayStyle.Flex;

    }


    private void RegisterMenuButtonsMain()
    {

        if (!hasAnySave)
            play = uIDocument.rootVisualElement.Q<Button>("Play");


        exit = uIDocument.rootVisualElement.Q<Button>("Exit");
        settings = uIDocument.rootVisualElement.Q<Button>("Settings");



        exit.RegisterCallback<MouseEnterEvent>(evt => OnButtonHoverEnter(exit));
        exit.RegisterCallback<MouseLeaveEvent>(evt => OnButtonHoverExit(exit));

        settings.RegisterCallback<MouseEnterEvent>(evt => OnButtonHoverEnter(settings));
        settings.RegisterCallback<MouseLeaveEvent>(evt => OnButtonHoverExit(settings));

        if (play != null)
            play.clicked += () => OnButtonClick(play);
        if (exit != null)
            exit.clicked += () => OnButtonClick(exit);
        if (settings != null)
            settings.clicked += () => OnButtonClick(settings);
    }

    private void RegisterMenuButtonsInGame()
    {

        exit = uIDocument.rootVisualElement.Q<Button>("Exit");
        settings = uIDocument.rootVisualElement.Q<Button>("Settings");
        _continue = uIDocument.rootVisualElement.Q<Button>("Continue");



        if (exit != null)
            exit.clicked += () => OnButtonClick(exit);
        if (settings != null)
            settings.clicked += () => OnButtonClick(settings);
        if (_continue != null)
            _continue.clicked += () => OnButtonClick(_continue);

    }

    private void registerBackButton()
    {
        var backButton = uIDocument.rootVisualElement.Q<Button>("Back");
        if (backButton != null)
        {
            backButton.RegisterCallback<MouseEnterEvent>(evt => OnButtonHoverEnter(backButton));
            backButton.RegisterCallback<MouseLeaveEvent>(evt => OnButtonHoverExit(backButton));
            backButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(backButton));
        }
    }

}