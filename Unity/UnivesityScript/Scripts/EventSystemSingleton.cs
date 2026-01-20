using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSingleton : MonoBehaviour
{
    private void Awake()
    {
        // Trova tutti gli EventSystem nella scena
        EventSystem[] systems = FindObjectsOfType<EventSystem>();

        foreach (var system in systems)
        {
            if (system != this.GetComponent<EventSystem>())
            {
                Debug.LogWarning("EventSystem duplicato trovato e rimosso: " + system.gameObject.name);
                Destroy(system.gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}
