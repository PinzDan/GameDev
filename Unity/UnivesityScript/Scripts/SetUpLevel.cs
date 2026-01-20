using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class SetUpLevel : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Tag del Player persistente")]
    public string playerTag = "Player";

    [Header("Punto di spawn del Player")]
    public string spawnPointTag = "SpawnPlayerPoint";
    public string checkpointSpawn = "CheckPointSpawn";

    [Header("Nome della scena di luce da caricare additivamente")]
    public string lightSceneName = "LightScene";

    [Header("textMesh save")]
    public GameObject salvataggio;


    [Header("Boss del livello")]
    public string bossName = "";
    private Dictionary<string, string> bossByScene = new Dictionary<string, string>()
    {
        {"Livello1", "Golem"},
        {"Livello2", "Turtle"},
        {"Livello3", "Dragon"}
    };




    [Header("Gestione Livello1")]

    public int livelliCompletati = -1;
    public GameObject gate2;
    public GameObject gate3;

    public bool checkPoint = false;




    private void Start()
    {


        if (GameManager.Instance.isDataLoaded)
            ApplyGameData();
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDataLoaded -= ApplyGameData;
    }

    private void Awake()
    {

        // Iscriviti all'evento di caricamento scena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Disiscriviti per evitare memory leak
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Collisioni.OnColliding -= enableCheckPoint;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {



        var animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        animator.enabled = true;

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().resetStats();

        // Ignora le scene additive
        if (mode == LoadSceneMode.Additive) return;


        StartCoroutine(TeleportPlayer());
        GameManager.Instance.OnDataLoaded += ApplyGameData;
        // Avvia coroutine per teletrasporto
        if (scene.name != "MappaTutorial")
        {

            Collisioni.OnColliding += enableCheckPoint;
            if (GameManager.Instance.checkPointEnabled) spawnPointTag = checkpointSpawn;

            StartCoroutine(WaitForCompleteLevel());
        }
        else
        {
            spawnPointTag = "SpawnPlayerPoint";

            if (GameManager.Instance.gameOver == true)
            {

                animator.Play("Idle_SwordShield");

                GameManager.Instance.gameOver = false;
            }

        }

        LightScene();
        salvataggio = GameObject.Find("SaveLoading");


    }

    private void ApplyGameData()
    {


        if (SceneManager.GetActiveScene().name == "MappaTutorial")
        {

            if (GameManager.Instance.GetTutorialStatus())
                Destroy(GameObject.FindWithTag("enemy"));


            gate2 = GameObject.Find("GateToLevel2");
            gate3 = GameObject.Find("GateToLevel3");
            Debug.Log("Fatto");
            livelliCompletati = GameManager.Instance.getLivelliCompletati();
            Debug.Log($"livelli: {GameManager.Instance.getCoins()}");
            switch (livelliCompletati)
            {
                case 1:
                    Debug.Log("Apro gate 2");
                    gate2.SetActive(false);
                    break;
                case 2:
                    gate3.SetActive(false);
                    break;
            }

            if (GameManager.Instance.isFirstLaunch == false)
            {
                GameObject menuCamera = GameObject.Find("MenuCamera");
                menuCamera.SetActive(false);
            }

        }


    }

    private IEnumerator TeleportPlayer()
    {
        // Aspetta un frame per assicurarti che tutto sia inizializzato
        yield return new WaitForEndOfFrame();

        // Trova il player persistente
        GameObject persistentPlayer = GameObject.FindGameObjectWithTag(playerTag);
        if (persistentPlayer == null)
        {
            Debug.LogError("SetUpLevel: Impossibile trovare il player con il tag " + playerTag);
            yield break;
        }

        // Trova il punto di spawn nella scena attuale
        GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);
        if (spawnPoint == null)
        {
            Debug.LogError("SetUpLevel: Impossibile trovare un oggetto con il tag 'SpawnPoint' nella scena " + SceneManager.GetActiveScene().name);
            yield break;
        }

        // Teletrasporta il player al punto di spawn
        persistentPlayer.transform.position = spawnPoint.transform.position;
        persistentPlayer.transform.rotation = spawnPoint.transform.rotation;

        Debug.Log("SetUpLevel: Player teletrasportato al punto di spawn nella scena " + SceneManager.GetActiveScene().name);

        yield return null;

        //LightScene();
    }


    private void LightScene()
    {
        SceneManager.LoadScene(lightSceneName, LoadSceneMode.Additive);
    }


    private void enableCheckPoint()
    {
        GameManager.Instance.checkPointEnabled = true;
        spawnPointTag = checkpointSpawn;
        GameManager.Instance.SalvaGioco(GameManager.Instance.getSlot());

    }
    private void disableCheckPoint()
    {
        GameManager.Instance.checkPointEnabled = false;
    }

    private IEnumerator WaitForCompleteLevel()
    {
        // Aspetta un frame per assicurarti che tutto sia inizializzato
        yield return new WaitForEndOfFrame();

        SetBossName(SceneManager.GetActiveScene().name);

        GameObject boss = GameObject.Find(bossName);

        if (boss != null)
        {

            yield return new WaitUntil(() => boss == null);

            disableCheckPoint();

            GameManager.Instance.CompletaLivello();

            GameManager.Instance.SalvaGioco(GameManager.Instance.getSlot());


            var cor = StartCoroutine(animazioneSalvataggio());


            yield return new WaitForSeconds(3f);

            StopCoroutine(cor);
            salvataggio.GetComponent<TMPro.TextMeshProUGUI>().enabled = false;

            GameManager.Instance.isFirstLaunch = false;

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().ApplyGameData();

            if (SceneManager.GetActiveScene().name == "Livello3")

                SceneManager.LoadScene("Credits");

            else
                SceneManager.LoadScene("MappaTutorial");

        }


    }


    private void SetBossName(string sceneName)
    {
        if (bossByScene.TryGetValue(sceneName, out string boss))
            bossName = boss;
        else
            bossName = "";
    }

    private IEnumerator animazioneSalvataggio()
    {
        int count = 0;

        var text = salvataggio.GetComponent<TMPro.TextMeshProUGUI>();
        text.enabled = true;

        text.text = "Salvataggio in corso";


        while (true)
        {
            if (count > 3) count = 1;

            string dots = new string('.', count);


            text.text += dots;

            yield return new WaitForSeconds(0.5f);
        }

    }
}
