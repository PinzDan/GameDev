using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPosition : MonoBehaviour
{
    public LayerMask groundLayer; // Layer del terreno per il raycast
    public float rayLength = 10f; // Lunghezza del raycast
    public float heightOffset = 0.1f; // Offset per mantenere il personaggio sopra il terreno

    void Start()
    {
        // Calcola la posizione del raycast
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength, groundLayer))
        {
            // Posiziona il personaggio sopra il punto di collisione del terreno
            Vector3 newPosition = hit.point + Vector3.up * heightOffset;
            transform.position = newPosition;
        }
    }
}
