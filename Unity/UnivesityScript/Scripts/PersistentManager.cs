using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class PersistentManager : MonoBehaviour
{
    public static PersistentManager instance;
    public GameObject playerGameObject;
    public UIDocument hotbarDocument;
    private HotbarUI hotbarUI;
    public GameObject dialoguePanel;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        playerGameObject = FindObjectOfType<CharacterPosition>()?.gameObject;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MappaTutorial")
        {
            NPCInteraction npcInteraction = FindObjectOfType<NPCInteraction>();
            if (npcInteraction != null)
            {
                npcInteraction.InitializeUIReferences();
            }

            // 🔥 Ricarica inventario persistente
            if (HotbarUI.Instance != null)
            {
                HotbarUI.Instance.caricaItems(true);
            }
        }
    }
}
