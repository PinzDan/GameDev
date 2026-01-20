using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ArenaTrigger : MonoBehaviour
{
    [Header("Roccia che chiude l’arena")]
    public GameObject blockExitObject;
    public Animator RockAnimator;
    public string rockTriggerName = "Animazione";

    [Header("Messaggio del Boss")]
    public string bossTextObjectName = "BossText";
    public string bossMessage = "Lo scontro ha inizio!";
    public float messageDuration = 3f;

    private TextMeshProUGUI bossMessageUI;
    private CanvasGroup canvasGroup;
    private bool triggered = false;

    void Start()
    {
        // Trova dinamicamente il BossText
        GameObject foundText = GameObject.Find(bossTextObjectName);
        if (foundText != null)
        {
            bossMessageUI = foundText.GetComponent<TextMeshProUGUI>();

            // Aggiungi CanvasGroup per fade-in/out se non esiste
            canvasGroup = bossMessageUI.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = bossMessageUI.gameObject.AddComponent<CanvasGroup>();

            bossMessageUI.text = "";
            canvasGroup.alpha = 0f;
        }
        else
        {
            Debug.LogWarning("BossText non trovato! Assicurati che sia attivo e correttamente nominato.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player"))
            return;

        if (blockExitObject != null)
            blockExitObject.SetActive(true);

        if (RockAnimator != null)
            RockAnimator.SetTrigger(rockTriggerName);

        if (bossMessageUI != null)
        {
            StopAllCoroutines(); // Per sicurezza se si rientra nel trigger
            StartCoroutine(ShowBossMessage());
        }

        triggered = true;
    }

    private IEnumerator ShowBossMessage()
    {
        bossMessageUI.text = bossMessage;

        // Fade-in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        yield return new WaitForSeconds(messageDuration);

        // Fade-out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        bossMessageUI.text = "";
    }
}
