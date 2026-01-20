using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    public CinemachineFreeLook playerCamera; // La telecamera che segue il giocatore
    public CinemachineVirtualCamera npcCamera; // La telecamera per l'NPC, se ne hai una

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DisablePlayerCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }
        // Se la tua telecamera non ha CinemachineInputProvider, questo metodo non causerà problemi
        CinemachineInputProvider inputProvider = playerCamera.GetComponent<CinemachineInputProvider>();
        if(inputProvider != null)
        {
            inputProvider.enabled = false;
        }
    }

    public void EnablePlayerCamera()
    {
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
        }
        
        CinemachineInputProvider inputProvider = playerCamera.GetComponent<CinemachineInputProvider>();
        if(inputProvider != null)
        {
            inputProvider.enabled = true;
        }
    }
    
    // Puoi aggiungere altri metodi qui per gestire altre telecamere
    public void EnableNPCCamera()
    {
        if(npcCamera != null)
        {
            DisablePlayerCamera(); // Assicurati di disabilitare l'altra telecamera
            npcCamera.enabled = true;
        }
    }

    public void DisableNPCCamera()
    {
        if(npcCamera != null)
        {
            npcCamera.enabled = false;
            EnablePlayerCamera(); // Riattiva la telecamera del giocatore
        }
    }
}