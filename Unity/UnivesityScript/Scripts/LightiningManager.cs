using UnityEngine;

public class LightingManager : MonoBehaviour
{
    private static bool exists = false;

    void Awake()
    {
        if (exists)
        {
            Destroy(gameObject);
            return;
        }

        exists = true;
        DontDestroyOnLoad(gameObject);
    }
}
