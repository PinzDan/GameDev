using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsManager : MonoBehaviour
{
    public RectTransform creditsContainer;

    public float scrollSpeed = 50f;

    public float startDelay = 2f;

    public string nextSceneName = "MainMenu";

    private bool isScrolling = false;

    GameObject persistent;

    void Awake()
    {
        persistent = GameObject.Find("PersistentManager");
        if (persistent != null)
            persistent.SetActive(false);
    }
    void Start()
    {

        if (creditsContainer != null)
        {
            Image panelImage = creditsContainer.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.color = new Color(0, 0, 0, 0);
            }

            creditsContainer.anchoredPosition = new Vector2(0, -(creditsContainer.rect.height / 2 + Screen.height / 2));
            StartCoroutine(StartCreditsScroll());
        }
        else
        {
            Debug.LogError("CreditsContainer non assegnato! Assegnalo nell'Inspector.");
        }
    }

    void Update()
    {
        if (isScrolling)
        {
            creditsContainer.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

            if (creditsContainer.anchoredPosition.y >= creditsContainer.rect.height)
            {
                isScrolling = false;
                StartCoroutine(LoadNextScene());
            }
        }
    }

    IEnumerator StartCreditsScroll()
    {
        yield return new WaitForSeconds(startDelay);
        isScrolling = true;
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(5f);
        persistent.SetActive(true);
        SceneManager.LoadScene("MappaTutorial");
    }
}
