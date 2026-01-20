using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityEngine.UIElements;

public class SideQuestInteraction : MonoBehaviour
{
    public UIDocument uiDocument;
    public string initialMessage = "Ciao, mi potresti aiutare a sconfiggere 2 di quei mostri? Ti ricompenserò con un artefatto che potenzierà i tuoi attacchi.";
    public string rewardMessage = "Ottimo lavoro, ecco la tua ricompensa! Prendi questa gemma e usala per diventare più forte.";
    public string questInProgressMessage = "Torna qui quando avrai sconfitto i mostri.";

    [Header("Reward Settings")]
    public ItemSO rewardItem;

    private VisualElement dialoguePanel;
    private Label dialogueText;
    private Button yesButton;
    private Button noButton;

    private GameObject player;
    private Moveset playerMoveset;
    private bool isInDialogue = false;
    private bool dialogueIsWaitingForExit = false;
    private float typingSpeed = 0.08f;
    private Coroutine typingCoroutine;
    private Camera mainCamera;
    private CinemachineFreeLook cineCam;
    private CinemachineInputProvider cineInput;
    private bool questAccepted = false;
    private int enemiesToKill = 2;
    private int enemiesKilled = 0;
    private bool isReady = false;
    private int selectedButtonIndex = 0;

    private Transform playerTransform;

    public GameObject[] enemies;
    public bool hasGivenReward = false;

    void Start()
    {
        InitializeAll();
    }

    private void InitializeAll()
    {
        if (PersistentManager.instance != null && PersistentManager.instance.playerGameObject != null)
        {
            player = PersistentManager.instance.playerGameObject;
            playerMoveset = player.GetComponent<Moveset>();
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player non trovato. Assicurati che PersistentManager esista e abbia il riferimento al player.");
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cineCam = mainCamera.GetComponent<CinemachineFreeLook>();
            if (cineCam != null)
            {
                cineInput = mainCamera.GetComponent<CinemachineInputProvider>();
            }
        }

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument non è assegnato nell'Inspector!");
            return;
        }

        var root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("UIDocument rootVisualElement è null! Controlla il tuo UXML.");
            return;
        }

        dialoguePanel = root.Q<VisualElement>("dialogue-panel");
        dialogueText = root.Q<Label>("dialogue-text");
        yesButton = root.Q<Button>("yes-button");
        noButton = root.Q<Button>("no-button");

        if (yesButton != null) yesButton.clicked += OnYesClicked;
        if (noButton != null) noButton.clicked += OnNoClicked;

        if (dialoguePanel != null) dialoguePanel.style.display = DisplayStyle.None;

        isReady = true;
        Debug.Log("SideQuestInteraction è stato inizializzato con successo.");
    }

    void Update()
    {
        if (isInDialogue && yesButton != null && noButton != null && !questAccepted)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedButtonIndex = 1 - selectedButtonIndex;
                UpdateSelectedButton();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                if (selectedButtonIndex == 0)
                {
                    OnYesClicked();
                }
                else
                {
                    OnNoClicked();
                }
            }
        }
    }

    private void UpdateSelectedButton()
    {
        if (selectedButtonIndex == 0)
        {
            yesButton.AddToClassList("selected-yes");
            noButton.RemoveFromClassList("selected-no");
        }
        else
        {
            yesButton.RemoveFromClassList("selected-yes");
            noButton.AddToClassList("selected-no");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isReady)
        {
            Debug.LogWarning("Il trigger è stato attivato prima che lo script fosse pronto. Riprova tra poco.");
            return;
        }

        if (other.CompareTag("Player") && !isInDialogue)
        {
            isInDialogue = true;
            StartCoroutine(StartDialogueAndLock());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !dialogueIsWaitingForExit)
        {
            CloseDialogue();
        }
    }

    IEnumerator StartDialogueAndLock()
    {
        StopPlayer();
        LockCamera();
        yield return null;
        ShowDialogue();
    }

    void OnYesClicked()
    {
        questAccepted = true;
        CloseDialogue();
        Debug.Log("Quest accettata! Sconfiggi 2 nemici.");
        StartCoroutine(waitQuestCompleted());
    }

    IEnumerator waitQuestCompleted()
    {
        // Attendi finché almeno un nemico esiste ancora nella scena
        while (enemies != null && System.Array.Exists(enemies, e => e != null))
        {
            yield return null; // aspetta il prossimo frame
        }
        hasGivenReward = true;


    }

    void OnNoClicked()
    {
        CloseDialogue();
        Debug.Log("Quest rifiutata.");
    }

    void ShowDialogue()
    {
        Debug.Log($"Mostrando dialogo. stato questAccepted: {questAccepted}, nemici sconfitti: {hasGivenReward}");
        if (typingCoroutine != null)
        {
            return;
        }

        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogError("Gli elementi UI del dialogo non sono stati trovati.");
            return;
        }

        // // 🔧 MODIFICA: Ora usiamo il metodo ShowHotbar di PersistentManager
        // if (PersistentManager.instance != null)
        // {
        //     PersistentManager.instance.ShowHotbar(false); // Nasconde la hotbar
        // }

        string messageToShow = "";

        if (!questAccepted)
        {
            messageToShow = initialMessage;
            if (yesButton != null) yesButton.style.display = DisplayStyle.Flex;
            if (noButton != null) noButton.style.display = DisplayStyle.Flex;
            selectedButtonIndex = 0;
            UpdateSelectedButton();
            typingCoroutine = StartCoroutine(TypeDialogue(messageToShow));
        }
        else if (questAccepted && hasGivenReward)
        {
            messageToShow = rewardMessage;
            if (yesButton != null) yesButton.style.display = DisplayStyle.None;
            if (noButton != null) noButton.style.display = DisplayStyle.None;
            StartCoroutine(GiveReward());
            typingCoroutine = StartCoroutine(TypeAndClose(messageToShow));
        }
        else
        {
            messageToShow = questInProgressMessage;
            if (yesButton != null) yesButton.style.display = DisplayStyle.None;
            if (noButton != null) noButton.style.display = DisplayStyle.None;
            typingCoroutine = StartCoroutine(TypeAndClose(messageToShow));
        }

        dialoguePanel.style.display = DisplayStyle.Flex;
    }

    IEnumerator TypeDialogue(string message)
    {
        dialogueText.text = "";
        foreach (char letter in message)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator TypeAndClose(string message)
    {
        dialogueIsWaitingForExit = true;
        yield return StartCoroutine(TypeDialogue(message));

        yield return new WaitForSeconds(1.5f);

        CloseDialogue();
        dialogueIsWaitingForExit = false;
    }

    public void CloseDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isInDialogue = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.style.display = DisplayStyle.None;
        }

        // 🔧 MODIFICA: Ora usiamo il metodo ShowHotbar di PersistentManager
        // if (PersistentManager.instance != null)
        // {
        //     PersistentManager.instance.ShowHotbar(true); // Mostra e aggiorna la hotbar
        // }

        StartCoroutine(UnlockCameraAndPlayer());
    }

    public void EnemyDefeated()
    {
        if (questAccepted && enemiesKilled < enemiesToKill)
        {
            enemiesKilled++;
            Debug.Log($"Nemico sconfitto. Nemici rimanenti: {enemiesToKill - enemiesKilled}");
        }
    }

    IEnumerator GiveReward()
    {
        if (InventoryManager.Instance != null && rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem, 1);

            // 🔧 MODIFICA: La hotbar viene aggiornata da CloseDialogue(), quindi non serve qui

            Debug.Log("Ricompensa data: Gemma aggiunta all'inventario!");
            yield return new WaitForSeconds(3.0f);
            //gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Impossibile dare la ricompensa. Controlla che InventoryManager e/o rewardItem siano impostati.");
        }
    }

    void StopPlayer()
    {
        if (player == null) return;

        if (playerMoveset != null)
        {
            playerMoveset.isMovementBlocked = true;
            playerMoveset.stopMovement();
            playerMoveset.enabled = false;
        }

        CharacterPosition charPosScript = player.GetComponent<CharacterPosition>();
        if (charPosScript != null) charPosScript.enabled = false;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Animator animazione = player.GetComponent<Animator>();
        if (animazione != null) animazione.SetFloat("Speed", 0);
    }

    void ResetPlayer()
    {
        if (player == null) return;

        if (playerMoveset != null)
        {
            playerMoveset.isMovementBlocked = false;
            playerMoveset.enabled = true;
        }

        CharacterPosition charPosScript = player.GetComponent<CharacterPosition>();
        if (charPosScript != null) charPosScript.enabled = true;
    }

    void LockCamera()
    {
        if (cineCam != null)
        {
            cineCam.m_Follow = null;
            cineCam.m_LookAt = null;
            cineCam.enabled = false;

            if (cineInput != null)
            {
                cineInput.enabled = false;
            }
        }
    }

    IEnumerator UnlockCameraAndPlayer()
    {
        StopPlayer();

        float elapsedTime = 0f;
        float duration = 1.0f;

        if (cineCam == null || player == null) yield break;

        Vector3 startPos = cineCam.transform.position;
        Quaternion startRot = cineCam.transform.rotation;

        Vector3 playerForward = player.transform.forward;
        Vector3 playerUp = player.transform.up;
        Vector3 targetPos = player.transform.position - playerForward * 5f + playerUp * 2f;
        Quaternion targetRot = Quaternion.LookRotation(playerForward, playerUp);

        while (elapsedTime < duration)
        {
            cineCam.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            cineCam.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cineCam.transform.position = targetPos;
        cineCam.transform.rotation = targetRot;

        cineCam.m_Follow = playerTransform;
        cineCam.m_LookAt = playerTransform;

        if (cineInput != null)
        {
            cineInput.enabled = true;
        }

        cineCam.enabled = true;

        ResetPlayer();

        yield return new WaitForSeconds(0.1f);
    }
}