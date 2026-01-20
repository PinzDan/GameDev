using System;
using UnityEngine;

public class Collisioni : MonoBehaviour
{

    public static event Action OnColliding;
    public bool hasCollided = false;


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("root"))
        {

            Debug.Log("Sto collidendo: " + other);
            OnColliding?.Invoke();
        }
    }


}
