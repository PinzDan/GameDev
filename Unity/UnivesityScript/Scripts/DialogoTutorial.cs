using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class DialogoTutorial : MonoBehaviour
{
    public enum TutorialStep { step0, step1, step2, step3, step4, step5, step6, step7, step8, completed }
    public StateManager stato;
    private TutorialStep tutorialState = TutorialStep.step0;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image enter;



    [Header("UI e comandi")]
    public GameObject UIEnemyTutorial;
    public GameObject UICombatGuide;
    public GameObject commands;
    public GameObject slime;
    public Camera enemiesCam;

    [Header("Keyboard")]
    [SerializeField] private Image wasd, frecce, u, esc, enterKey, shift;
    [Header("Mouse")]
    [SerializeField] private Image clickdestro1, clickdestro2, Rotella;
    [Header("Pozioni")]
    [SerializeField] private Image Potion1, Potion2;

    private bool hasCollided = false;
    private bool enemyStatus = false;

    private float typingSpeed = 0.00000005f;

    private int step = 0;

    private void OnEnable()
    {
        Collisioni.OnColliding += () => hasCollided = true;
        Events.OnEnemyEvent += () => enemyStatus = true;
    }

    void Start()
    {
        if (tutorialState == TutorialStep.completed)
        {
            Destroy(this);
            return;
        }

        StartCoroutine(playTutorial());
    }

    private IEnumerator playTutorial()
    {
        yield return new WaitForSeconds(1f);

        while (tutorialState != TutorialStep.completed)
        {
            yield return StartCoroutine(EseguiStep(step));
            step++;
        }
        GameManager.Instance.setTutorial(true);
        yield return null;
        Destroy(gameObject);
    }

    private IEnumerator EseguiStep(int step)
    {
        string message = "";
        bool waitForInput = true;

        switch (step)
        {
            case 0:
                message = "Ah... eccoti finalmente. Il destino ha scelto il suo campione...";
                yield return StartCoroutine(TypeTextCoroutine(message));

                break;
            case 1:
                message = "Prima di tutto... impariamo a muoverci nel mondo.";
                yield return StartCoroutine(TypeTextCoroutine(message));

                waitForInput = false;

                frecce.gameObject.SetActive(true);
                wasd.gameObject.SetActive(true);

                commands.SetActive(true);

                yield return new WaitUntil(() =>
                    Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0);

                yield return new WaitUntil(() => hasCollided);

                frecce.gameObject.SetActive(false);
                wasd.gameObject.SetActive(false);
                commands.SetActive(false);
                break;
            case 2:
                message = "Guarda... proprio là. Vedi quella figura?\n\nNon aver paura!";
                enemiesCam.gameObject.SetActive(true);
                commands.GetComponent<Commands>().dontWalk();
                commands.SetActive(false);

                yield return StartCoroutine(TypeTextCoroutine(message));




                commands.SetActive(true);
                UIEnemyTutorial.SetActive(true);

                break;
            case 3:
                enemiesCam.gameObject.SetActive(false);

                message = "È il momento. Colpisci!";
                yield return StartCoroutine(TypeTextCoroutine(message));

                waitForInput = false;

                clickdestro1.enabled = true;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse1));
                clickdestro1.enabled = false;
                break;
            case 4:
                message = "Lo sento... dentro di te c’è più potere.";
                yield return StartCoroutine(TypeTextCoroutine(message));

                waitForInput = false;
                clickdestro2.enabled = true;
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) || slime == null);
                if (slime != null) yield return new WaitUntil(() => slime == null);

                clickdestro2.enabled = false;
                break;
            case 5:
                message = "Ben fatto! La tua prima vittoria...";
                yield return StartCoroutine(TypeTextCoroutine(message));

                break;
            case 6:
                message = "Se torni indietro troverai il mercante...\n" +
                "usa 'U'per usare gli oggetti e il mouse per selezionarli";
                yield return StartCoroutine(TypeTextCoroutine(message));

                u.enabled = true;
                Rotella.enabled = true;
                break;
            case 7:
                message = "Troverai oggetti utili, usali con cura...";
                yield return StartCoroutine(TypeTextCoroutine(message));

                Potion1.enabled = true;
                Potion2.enabled = true;
                break;
            case 8:
                message = "Premendo 'Shift' puoi Correre!";
                yield return StartCoroutine(TypeTextCoroutine(message));
                shift.enabled = true;
                break;
            case 9:
                message = "Premendo 'ESC' puoi mettere in pausa il gioco e salvare";
                yield return StartCoroutine(TypeTextCoroutine(message));

                esc.enabled = true;
                break;
            case 10:
                message = "Adesso segui i tre sentieri... da qui inizia la tua avventura!";
                yield return StartCoroutine(TypeTextCoroutine(message));

                UICombatGuide.SetActive(false);
                UIEnemyTutorial.SetActive(false);
                tutorialState = TutorialStep.completed;
                break;
        }



        // Aspetta input solo se richiesto
        if (waitForInput)
        {
            enterKey.enabled = true;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }


        // Reset UI comandi
        enterKey.enabled = false;
        u.enabled = false;
        Rotella.enabled = false;
        esc.enabled = false;
        Potion1.enabled = false;
        Potion2.enabled = false;
        shift.enabled = false;
    }

    private IEnumerator TypeTextCoroutine(string message)
    {
        text.text = "";
        foreach (char c in message)
        {
            text.text += c;

            float delay = typingSpeed;
            if (c == '.' || c == '!' || c == '?') delay *= 6;
            else if (c == ',' || c == ';') delay *= 3;

            yield return new WaitForSeconds(delay);
        }
    }
    private IEnumerator AspettaEnter()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
    }
}
