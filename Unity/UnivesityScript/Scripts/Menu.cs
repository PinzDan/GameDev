using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public StateManager stato;
    [SerializeField] private Button start;
    [SerializeField] private Button options;
    [SerializeField] private Button credits;

    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private Camera menuCamera;

    

    void Start()
    {
        
        start.onClick.AddListener(OnStartButtonClick);
    }

    public void OnStartButtonClick(){
        
        stato.Resume();
        if (freeLookCamera != null)
        {
            Debug.Log("PRemuto");
            freeLookCamera.enabled = true;
            
        }

        if (menuCamera != null)
        {
            menuCamera.gameObject.SetActive(false); // Disabilita la camera principale
        }
        
        gameObject.SetActive(false);   
    }
}
