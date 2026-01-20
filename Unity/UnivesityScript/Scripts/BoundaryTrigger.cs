using UnityEngine;
using Cinemachine;
using TMPro;
using System.Collections;

public class BoundaryTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera warningCamera;   // La telecamera di avviso (Cinemachine Virtual Camera)
    public Canvas boundaryCanvas;                    // Riferimento al canvas BoundaryCanvas
    public GameObject boundaryCamera;                // Riferimento al GameObject BoundaryCamera (il punto di riferimento per la telecamera)
    public float transitionSpeed = 2.6f;             // Velocità di transizione della telecamera
    public float warningDuration = 4f;               // Durata del messaggio di avviso e telecamera attiva (in secondi)
    public float flashSpeed = 1.5f;                  // Velocità del lampeggio del testo

    private bool isPlayerOutOfBounds = false;
    private TextMeshProUGUI warningMessage;          // Messaggio di avviso
    private Coroutine warningCoroutine;
    
    private void Start()
    {
        // Verifica e assegna warningMessage
        if (boundaryCanvas != null)
        {
            warningMessage = boundaryCanvas.GetComponentInChildren<TextMeshProUGUI>();

            if (warningMessage == null)
            {
                Debug.LogError("BoundaryTrigger: 'BoundaryCanvas' non contiene un TextMeshProUGUI in nessuno dei suoi oggetti figli. Assicurati che 'BoundaryText' sia un figlio di 'BoundaryCanvas' e che abbia il componente TextMeshProUGUI.");
            }
            else
            {
                Debug.Log("BoundaryTrigger: warningMessage trovato e assegnato con successo.");
            }
        }
        else
        {
            Debug.LogError("BoundaryTrigger: boundaryCanvas non è assegnato. Assegnalo nell'Inspector.");
        }

        if (warningCamera == null)
        {
            Debug.LogError("BoundaryTrigger: warningCamera non è assegnata. Assegnala nell'Inspector.");
        }
        else
        {
            Debug.Log("BoundaryTrigger: warningCamera assegnata correttamente.");
        }

        // Nascondi il messaggio, lo sfondo e disattiva la telecamera di avviso inizialmente, se sono stati trovati
        if (warningMessage != null) 
        {
            warningMessage.gameObject.SetActive(false);
            warningMessage.alpha = 0f;  // Assicurati che il testo parta invisibile
        }

        if (warningCamera != null) 
        {
            warningCamera.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOutOfBounds = true;

            // Attiva il messaggio di avviso
            if (warningMessage != null)
            {
                warningMessage.text = "STAI USCENDO DAL SENTIERO,TORNA INDIETRO!";
                warningMessage.gameObject.SetActive(true);
                StartCoroutine(FlashText());  // Avvia il lampeggio del testo
            }

            // Attiva la telecamera di avviso
            if (warningCamera != null)
            {
                warningCamera.gameObject.SetActive(true);

                // Imposta il LookAt della telecamera per guardare il punto di riferimento specifico per questa zona
                if (boundaryCamera != null)
                {
                    warningCamera.LookAt = boundaryCamera.transform;
                    warningCamera.Follow = boundaryCamera.transform;
                }
            }

            // Avvia la coroutine per tornare alla normalità dopo il tempo specificato
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);  // Ferma eventuali coroutine precedenti
            }
            warningCoroutine = StartCoroutine(ReturnToPathAfterDelay(warningDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Se il giocatore esce dal collider, riporta tutto alla normalità immediatamente
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }
            ReturnToPath();
        }
    }

    private IEnumerator FlashText()
    {
        // Iniziamo il lampeggio del testo in modo fluido
        float elapsedTime = 0f;
        while (isPlayerOutOfBounds)
        {
            elapsedTime += Time.deltaTime * flashSpeed;  // Aumenta il tempo con la velocità del lampeggio

            // Imposta l'alpha del testo in modo che vada da 1 (opaco) a 0 (trasparente)
            float alpha = Mathf.PingPong(elapsedTime, 1f); // Il PingPong fa oscillare tra 0 e 1
            warningMessage.alpha = alpha;  // Cambia la trasparenza del testo

            yield return null;  // Continua nel prossimo frame
        }

        // Assicura che il messaggio rimanga visibile alla fine
        warningMessage.alpha = 1f;
    }

    private IEnumerator ReturnToPathAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPath();
    }

    private void ReturnToPath()
    {
        isPlayerOutOfBounds = false;

        // Nascondi il messaggio di avviso
        if (warningMessage != null)
        {
            warningMessage.gameObject.SetActive(false);
            warningMessage.alpha = 0f;  // Nascondi il testo in modo fluido
        }

        // Disattiva la telecamera di avviso
        if (warningCamera != null)
        {
            warningCamera.gameObject.SetActive(false);
        }
    }
}
