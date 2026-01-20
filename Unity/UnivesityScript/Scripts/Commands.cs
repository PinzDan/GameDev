using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Lo script si occupera di gestire gli input emessi dal giocatore.
/*
    Comandi del personaggio: 
        Tasti di movimento: 
            WASD E FRECCE DIREZIONALI
        
        Corsa:
            tasti di movimento + SHIFT;
        Attacchi:
            Tasto sinistro -> attaccco normale
            Tasto desto -> attacco speciale
        
    Altri comandi:
        Pausa:
            ESC;
*/


public class Commands : MonoBehaviour
{
    private Moveset playerMoveset;
    public GameObject Player;


    private float horizontalInput;
    private float verticalInput;
    private bool isShifted = false;
    private bool mosso = false;

    private typeAttack tipo;

    [SerializeField] GameObject gamestate;
    [SerializeField] GameObject PlayerState;

    [SerializeField] private GameObject sword;

    private PlayerStateManager playerStateManager;

    private Coroutine startCourotine;

    void OnEnable()
    {
        PlayerAttackSubject.OnAttackStateEnter += OnAttackEnter;
        PlayerAttackSubject.OnAttackStateExit += OnAttackExit;

        startCourotine = null; // Inizializza la coroutine per gli attacchi
    }

    void OnDisable()
    {
        PlayerAttackSubject.OnAttackStateEnter -= OnAttackEnter;
        PlayerAttackSubject.OnAttackStateExit -= OnAttackExit;
    }


    void Start()
    {
        PlayerState = GameObject.Find("PlayerManager");
        playerStateManager = PlayerState.GetComponent<PlayerStateManager>();
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        playerMoveset = Player.GetComponent<Moveset>();



    }

    void FixedUpdate()
    {
        movementControl();
    }
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (startCourotine == null)
            startCourotine = StartCoroutine(callAttack());
        mettiPausa();
        cancellaSlot();


    }

    private void OnAttackEnter()
    {
        // playerStateManager.updatePlayerState(global::PlayerState.Attack);
        playerStateManager.setAttack();
    }

    private void OnAttackExit()
    {

        playerStateManager.unsetAttack();
        Debug.Log($"unset: {playerStateManager.getStato()}");
        // playerStateManager.updatePlayerState();
    }

    public typeAttack inputMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return typeAttack.Special; // Attacco speciale
        }
        else if (Input.GetMouseButtonDown(1))
        {

            return typeAttack.Normal; // Attacco normale
        }
        return typeAttack.none;
    }

    public IEnumerator callAttack()
    {
        // Ottieni il tipo di attacco

        yield return new WaitUntil(() =>
            {
                tipo = inputMouse(); // Ottieni il tipo di attacco
                return tipo != typeAttack.none;
            });

        WeaponsTrigger weaponsTrigger = sword.GetComponent<WeaponsTrigger>();
        if (tipo == typeAttack.Special)
            weaponsTrigger.dannoBase = GameManager.Instance.getDannoBase() * 1.5f;
        else
            weaponsTrigger.dannoBase = GameManager.Instance.getDannoBase();
        playerMoveset.inputAttack(tipo);
        startCourotine = null; // Rendi di nuovo possibile l'invocazione della coroutine


    }

    public void movementControl()
    {

        isShifted = Input.GetKey(KeyCode.LeftShift);

        if (horizontalInput != 0f || verticalInput != 0f)
        {
            playerStateManager.setMove();
            muovi(horizontalInput, verticalInput, isShifted);
            ruota(horizontalInput, verticalInput);

            mosso = true;
        }
        else if (mosso)
        {
            playerStateManager.setIdle();
            playerMoveset.stopMovement();
            playerMoveset.itsRun();
            mosso = false;
        }
    }



    public void muovi(float inputX, float inputY, bool run)
    {
        playerMoveset.moveSet(inputX, inputY, run);
    }
    public void ruota(float inputX, float inputY)
    {
        playerMoveset.rotazione(inputX, inputY);
    }



    /** PAUSA */
    public void mettiPausa()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StateManager state = gamestate.GetComponent<StateManager>();
            if (!state.itsPaused())
                state.Pause();
            else
                state.Resume();
        }
    }

    public bool dontWalk()
    {
        Player.GetComponent<Animator>().SetFloat("Speed", 0f);
        playerMoveset.stopMovement(); // Vector3.zero; evita che continui a spostarsi il player
        return true;
    }



    public void cancellaSlot()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SaveSystem.CancellaSlot(1);
            SaveSystem.CancellaSlot(2);
            SaveSystem.CancellaSlot(3);
        }

    }

}
