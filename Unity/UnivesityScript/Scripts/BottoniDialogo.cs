using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class BottoniDialogo : MonoBehaviour
{

    public GameObject player;

    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [SerializeField] private Color buttonColor = Color.black;
    [SerializeField] private GameObject targetPoint1;
    public Image arrow;


    private float selectedOpacity = 1f;
    private float defaultOpacity = 0.5f;

    private bool isMovingBackward = true;


    private Button selectedButton;
    
    // Start is called before the first frame update
    void Start()
    {

        
        setArrowPosition(yesButton.transform.position);
        setButtonSelected(yesButton, true);
        setButtonSelected(noButton, false);
        selectedButton = yesButton;
        
        //yesButton.onClick.AddListener(yesClicked);
        noButton.onClick.AddListener(noClicked);   
    }

    void Update(){
         if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ToggleSelection();
            Debug.Log(selectedButton);
        }
        
        if(Input.GetKeyDown(KeyCode.Return))
            clickButton(selectedButton);
    
    }

    void clickButton(Button button){
            button.onClick.Invoke();
    }

    // void stopPlayer(){
    //     Moveset moveScript = player.GetComponent<Moveset>();
    //     moveScript.enabled = false;

    //     Rigidbody rb = player.GetComponent<Rigidbody>();
    //     rb.velocity = Vector3.zero;
    //     // CharacterPosition position = player.GetComponent<CharacterPosition>();
    //     // position.enabled = false;

    //     Animator animazione = player.GetComponent<Animator>();
    //     animazione.SetFloat("Speed", 0);
    // }

    void resetPlayer(){
        // Moveset moveScript = player.GetComponent<Moveset>();
        // moveScript.enabled = true;
        exitPlayer();
    }

    void exitPlayer(){
        Rigidbody rb = player.GetComponent<Rigidbody>();
       
        StartCoroutine(MoveForward(rb));
    }

     private void RotatePlayer()
    {
        Transform tr = player.GetComponent<Transform>();
        tr.Rotate(0,90f,0);
    }

    private IEnumerator MoveForward(Rigidbody rb){
        while(Vector3.Distance(player.transform.position, targetPoint1.transform.position)>0.1f){
            Vector3 direction = (targetPoint1.transform.position - player.transform.position).normalized;
            // Muovi in avanti
            rb.MovePosition(player.transform.position + direction * 5f * Time.deltaTime);
 
            yield return null;
        }
            
            
            player.transform.position = targetPoint1.transform.position;
            RotatePlayer();
    }


    void noClicked(){
        resetPlayer();
    }
    void ToggleSelection()
    {
        if (selectedButton == yesButton)
        {
            setButtonSelected(yesButton, false);
            setButtonSelected(noButton, true);
            selectedButton = noButton; // Cambia la selezione
            setArrowPosition(selectedButton.transform.position); // Sposta la freccia
        }
        else if (selectedButton == noButton)
        {
            setButtonSelected(noButton, false);
            setButtonSelected(yesButton, true);
            selectedButton = yesButton; // Cambia la selezione
            setArrowPosition(selectedButton.transform.position); // Sposta la freccia
        }
    }

    void setArrowPosition(Vector3 buttonPosition){

        arrow.transform.position = buttonPosition - new Vector3(50, 0, 0);

    }

    void setButtonSelected(Button button, bool isSelected){
        
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            Color textColor = buttonText.color;

            buttonText.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
            textColor.a = isSelected ? selectedOpacity : defaultOpacity ; // Cambia l'opacità del testo
            buttonText.color = textColor;
        }
    }
}

    
