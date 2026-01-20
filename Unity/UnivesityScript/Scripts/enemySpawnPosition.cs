using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawnPosition : MonoBehaviour
{
    // Start is called before the first frame update


    void Start()
    {
        Collider genitoreCollider = GetComponent<Collider>();
        Bounds bounds = genitoreCollider.bounds;

        foreach (Transform figlioTransform in transform)
        {

            Vector3 posizioneCasuale = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );


            figlioTransform.position = posizioneCasuale;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
