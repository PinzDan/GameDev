using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using System.Collections;
using UnityEngine.UIElements;

public class NPCInteraction : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public string npcMessage = "Ciao, vorresti comprare qualcosa?";
    [SerializeField] private UnityEngine.UI.Button yesButton;
    [SerializeField] private UnityEngine.UI.Button noButton;

    public UIDocument shopDocument;
    public ShopUI shopUI;
    private VisualElement shopRoot;
    private VisualElement backgroundDim;

    public UIDocument hotbarDocument;

    [SerializeField] private GameObject commandsGameObject;

    private CinemachineFreeLook cineCam;
    private CinemachineInputProvider cineInput;

    private float typingSpeed = 0.08f;
    private bool isInteractionActive = false;

    void Awake()
    {
        commandsGameObject = GameObject.Find("Commands");
        if (commandsGameObject == null)
        {
            Debug.LogError("Il GameObject 'Commands' non è stato trovato nella scena!");
        }

        cineCam = FindObjectOfType<CinemachineFreeLook>();
        if (cineCam != null)
        {
            cineInput = cineCam.GetComponent<CinemachineInputProvider>();
        }
    }

    public void InitializeUIReferences()
    {
        Debug.Log("Inizializzazione riferimenti UI di NPCInteraction...");

        dialoguePanel = PersistentManager.instance.dialoguePanel;
        if (dialoguePanel == null)
        {
            Debug.LogError("DialoguePanel non trovato nel PersistentManager!");
            return;
        }

        dialogueText = dialoguePanel.transform.FindDeepChild("MarketText2")?.GetComponent<TMP_Text>();
        yesButton = dialoguePanel.transform.FindDeepChild("Yes")?.GetComponent<UnityEngine.UI.Button>();
        noButton = dialoguePanel.transform.FindDeepChild("No")?.GetComponent<UnityEngine.UI.Button>();

        shopUI = GameObject.FindObjectOfType<ShopUI>();
        if (shopUI != null)
        {
            shopDocument = shopUI.GetComponent<UIDocument>();
            if (shopDocument != null && shopDocument.rootVisualElement != null)
            {
                shopRoot = shopDocument.rootVisualElement.Q<VisualElement>("shop-root");
                backgroundDim = shopDocument.rootVisualElement.Q<VisualElement>("background-dim");
            }
        }

        HotbarUI hotbarUI = GameObject.FindObjectOfType<HotbarUI>();
        if (hotbarUI != null)
        {
            hotbarDocument = hotbarUI.GetComponent<UIDocument>();
        }

        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(OnYesClicked);
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(OnNoClicked);
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (shopRoot != null) shopRoot.style.display = DisplayStyle.None;
        if (backgroundDim != null) backgroundDim.style.display = DisplayStyle.None;
        if (shopUI != null) shopUI.enabled = false;
        if (hotbarDocument != null) hotbarDocument.gameObject.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInteractionActive)
        {
            isInteractionActive = true;
            StartCoroutine(StartInteractionCoroutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInteractionActive)
        {
            StartCoroutine(CloseDialogue());
        }
    }

    IEnumerator StartInteractionCoroutine()
    {
        yield return null;
        DisablePlayerControls();
        StartCoroutine(ShowDialogue());
    }

    private void OnYesClicked()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (hotbarDocument != null) hotbarDocument.gameObject.SetActive(false);

        ShowShop(true);
    }

    private void OnNoClicked()
    {
        StartCoroutine(CloseDialogue());
    }

    IEnumerator ShowDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (hotbarDocument != null) hotbarDocument.gameObject.SetActive(false);

        if (dialogueText != null)
        {
            dialogueText.text = "";
            yield return StartCoroutine(TypeDialogue(npcMessage));
        }
    }

    private void ShowShop(bool state)
    {
        if (shopRoot != null) shopRoot.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        if (backgroundDim != null) backgroundDim.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        if (shopUI != null) shopUI.enabled = state;

        if (!state)
        {
            // 🔥 Salva inventario dopo la chiusura dello shop
            InventoryManager.Instance.SaveInventoryToGameManager();
            HotbarUI.Instance.RefreshAllSlots();
        }

        if (hotbarDocument != null)
            hotbarDocument.gameObject.SetActive(!state);
    }

    public IEnumerator CloseDialogue()
    {
        if (shopRoot != null && shopRoot.style.display == DisplayStyle.Flex)
        {
            ShowShop(false);
        }

        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            string goodbyeMessage = "A presto!";
            if (dialogueText != null) dialogueText.text = "";
            yield return StartCoroutine(TypeDialogue(goodbyeMessage));
            yield return new WaitForSeconds(1.0f);
            dialoguePanel.SetActive(false);
        }

        if (hotbarDocument != null)
            hotbarDocument.gameObject.SetActive(true);

        isInteractionActive = false;
        EnablePlayerControls();
    }

    private void DisablePlayerControls()
    {
        if (commandsGameObject != null) commandsGameObject.SetActive(false);
        if (cineCam != null) cineCam.enabled = false;
        if (cineInput != null) cineInput.enabled = false;
    }

    private void EnablePlayerControls()
    {
        if (commandsGameObject != null) commandsGameObject.SetActive(true);
        if (cineCam != null) cineCam.enabled = true;
        if (cineInput != null) cineInput.enabled = true;
    }

    IEnumerator TypeDialogue(string message)
    {
        if (dialogueText == null) yield break;
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
