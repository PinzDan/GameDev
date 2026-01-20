using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
public class EnemyPosition : MonoBehaviour
{

    private Quaternion originalRotation;

    private bool battle = false;

    private Transform player;
    private GameObject HealthBar;

    private Transform enemyCanvasIcon;
    public CinemachineFreeLook mainCamera;

    [SerializeField] private GameObject PlayerState;
    [SerializeField] private bool enemyState;

    private PlayerStateManager state;

    // Start is called before the first frame update
    void Start()
    {

        PlayerState = GameObject.Find("PlayerManager");
        state = PlayerState.GetComponent<PlayerStateManager>();
        // SphereCollider range = gameObject.AddComponent<SphereCollider>();
        // range.radius = 7f;
        // range.center = new Vector3(0, 0, 0);
        // range.isTrigger = true;
        gameObject.tag = "enemy";
        enemyState = false;


        player = GameObject.Find("RPGHeroPolyart").transform;
        originalRotation = transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        if (battle)
        {
            Debug.Log("Dentro Battle");
            enemyState = true;
            chase();
        }
    }


    void chase()
    {
        enemyCanvasIcon = transform.Find("enemyCanvas");

        player = GameObject.Find("RPGHeroPolyart").transform;
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Blocca la componente Y per evitare inclinazioni

        // Se la direzione non è zero, procedi con la rotazione
        if (direction.sqrMagnitude > 0.01f) // Usa sqrMagnitude per una verifica più veloce
        {
            // Calcola l'angolo di rotazione desiderato
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            Quaternion playerRotation = Quaternion.LookRotation(transform.position - player.position);

            // Ruota gradualmente verso il target, senza troppa fretta
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 200f);
            player.rotation = Quaternion.Slerp(player.rotation, playerRotation, Time.deltaTime * 200f);

            enemyCanvasIcon.Find("angry").gameObject.SetActive(true);
        }



    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            changeStatusBattle();
            state.setCombat();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            resetEnemy();
            changeStatusBattle();
            state.Reset();
        }

    }

    public void changeStatusBattle()
    {
        battle = !battle;
    }

    private void resetEnemy()
    {
        transform.rotation = originalRotation;
        enemyCanvasIcon.Find("angry").gameObject.SetActive(false);
    }

    public bool getState()
    {
        return this.enemyState;
    }

}
