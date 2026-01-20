using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 0.125f;
    public float initialFOV = 30f;      // Field of View iniziale
    public float targetFOV = 60f;       // Field of View finale
    public float fovSmoothSpeed = 5f;


    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = initialFOV;  // Imposta il FOV iniziale
    }

    // Start is called before the first frame update
    void LateUpdate()
    {
        // Calcola la posizione desiderata
        Vector3 desiredPosition = target.position + offset;
        // Interpolazione per movimento fluido
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Assicurati che la camera guardi sempre il personaggio
        transform.LookAt(target);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothSpeed);
    }
}
