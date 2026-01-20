using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public enum SceneType
{
    Level1,
    Level2,
    Level3
}

public class TeletrasportoPlayer : MonoBehaviour
{
    public SceneType sceneType;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (sceneType)
            {
                case SceneType.Level1:
                    SceneManager.LoadScene("Livello1");
                    break;
                case SceneType.Level2:
                    Debug.Log("Loading Level 2");
                    SceneManager.LoadScene("Livello2");
                    break;
                case SceneType.Level3:
                    SceneManager.LoadScene("Livello3");
                    break;
                default:
                    Debug.LogError("Unknown scene type!");
                    break;
            }
        }
    }

}