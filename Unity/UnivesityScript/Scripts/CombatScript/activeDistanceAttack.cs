using UnityEngine;

public class activeDistanceAttack : MonoBehaviour
{
    // Inizializzazione della classepublic GameObject target;
    public GameObject target;
    public void AttivaOggetto()
    {
        Debug.Log("Attivazione dell'oggetto: " + target.name);
        target.SetActive(true);
    }
}