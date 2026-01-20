
using System;
using System.Collections;
using UnityEngine;

public class Events : MonoBehaviour
{
    public bool checkEnemy = false;
    public static event Action OnEnemyEvent;
    void Start()
    {

        switch (gameObject.tag)
        {
            // case "Player":
            //     PlayerEvents.OnPlayerDeath += PlayerEvents_OnPlayerDeath;
            //     break;
            case "enemy":
                StartCoroutine(WaitUntilUntagged());
                break;
            // case "Item":
            //     ItemEvents.OnItemCollected += ItemEvents_OnItemCollected;
            //     break;

            default:
                Debug.LogWarning("No events registered for this object: " + gameObject.name);
                break;
        }
    }



    // enemy event //
    private IEnumerator WaitUntilUntagged()
    {
        Debug.Log("Waiting for enemy to be untagged: " + gameObject.name);
        yield return new WaitUntil(() => gameObject.tag == "Untagged");
        Debug.Log("Enemy untagged, registering events: " + gameObject.name);
        handleEnemyEvents();
    }
    void handleEnemyEvents()
    {
        Debug.Log("Enemy events registered for: " + gameObject.name);
        OnEnemyEvent?.Invoke();
        //Destroy(this.gameObject);
    }

    // ******* //
}
