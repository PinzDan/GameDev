using UnityEngine;
using Cinemachine;
using System;
using gameSpaces.enemies;



[Flags]
public enum PlayerState
{
    None = 0,
    Idle = 1 << 1, //0001
    Combat = 1 << 2, //0010
    Attack = 1 << 3, //0100
    Move = 1 << 4, //1000
    Normal = 1 << 5, //10000


}

public class PlayerStateManager : MonoBehaviour
{

    [SerializeField] private PlayerState _stato;
    public PlayerState getStato()
    {
        return _stato;
    }
    // tramite invoke segnala il cambiamento di stato all'audio controller
    private PlayerState stato
    {
        get => _stato;
        set
        {
            if (_stato != value)
            {
                var previous = _stato;
                var addedFlags = value & ~_stato; // Prende solo i flag nuovi

                _stato = value;

                if (addedFlags != PlayerState.None)
                {
                    OnPlayerStateChanged?.Invoke(addedFlags); // Invia solo i nuovi stati
                    //Debug.Log($"Stati aggiunti: {addedFlags}");
                }

                //Debug.Log($"Stato cambiato da {previous} a {_stato}");
            }
        }
    }

    [SerializeField] private Moveset movementScript;
    //[SerializedField] Script CombatScript;
    // [SerializeField] private PlayerStats stats;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject CombatCamera;
    [SerializeField] private GameObject player;



    private Moveset component;
    private PlayerStats stats;

    public event Action<PlayerState> OnPlayerStateChanged;

    // Start is called before the first frame update
    void Awake()
    {

        /* Qui vengono inizializati gli script del personaggio*

            1- Aggiunta componente per le statistiche
            2- Aggiunta componente per i moveset
            3- inizializzo il moveset
            4- setto lo stato ad Idle (stato base)
        */
        stats = player.AddComponent<PlayerStats>();
        stats.OnPlayerHit += getHit; // Associa l'evento getHit allo script PlayerStats
        movementScript = player.AddComponent<Moveset>();
        movementScript.MovesetIni(stats, mainCamera);


        component = player.GetComponent<Moveset>();


        /* 2- Settaggio dei moveset */
        SetOnlyState(PlayerState.Idle);
    }




    // ====== Funzioni per la gestione degli stati del player ====== //

    private void AddState(PlayerState state) => stato |= state;
    private void RemoveState(PlayerState state) => stato &= ~state;
    private void SetOnlyState(PlayerState state) => stato = state;
    public bool HasState(PlayerState state) => (stato & state) != 0;


    /* Basic Function */


    public void reset()
    {
        SetOnlyState(PlayerState.Idle);
    }
    public void setIdle()
    {
        AddState(PlayerState.Idle);

        RemoveState(PlayerState.Move); // Rimuove lo stato Move

        component.enabled = false;
        //CombatCamera.setActive(false);
    }
    public void setCombat()
    {

        AddState(PlayerState.Combat);
        RemoveState(PlayerState.Normal); // Rimuove lo stato Normal
        trovaNemicoAttivo();
        CombatCamera.SetActive(true);

    }

    public void setMove()
    {
        AddState(PlayerState.Move);
        RemoveState(PlayerState.Idle); // Rimuove lo stato Idle
        if (!component.enabled) component.enabled = true;

    }

    public void setNormal()
    {
        stato = PlayerState.Normal;
    }

    //Aggiunge lo stato attack allo stato corrente del player
    public void setAttack()
    {
        AddState(PlayerState.Attack);
    }

    public void unsetAttack()
    {
        RemoveState(PlayerState.Attack); // Rimuove lo stato Attack
    }

    public void trovaNemicoAttivo()
    {

        EnemyType enemy = GameObject.FindWithTag("activeEnemy").GetComponent<Enemy>().getEnemyType();
        Transform target = GameObject.FindGameObjectWithTag("activeEnemy").transform;

        CinemachineVirtualCamera cam = CombatCamera.GetComponent<CinemachineVirtualCamera>();

        if (enemy == EnemyType.Golem)
            cam.m_Lens.FieldOfView = 100;
        else
            cam.m_Lens.FieldOfView = 80;

        cam.LookAt = target;

    }


    public void Reset()
    {
        CombatCamera.SetActive(false);
        SetOnlyState(PlayerState.Normal);
    }

    /*********/

    public void getHit(EnemyType type, float enemyAttackDamage)
    {

        if (HasState(PlayerState.Combat))
        {
            Debug.Log($"Player colpito da un nemico: {type}");
            Animator animator = player.GetComponent<Animator>();

            if (type == EnemyType.Golem || type == EnemyType.Dragon)
                animator.SetTrigger("getHitHeavy");
            else
                animator.SetTrigger("getHit");

            stats.getHit(enemyAttackDamage);
        }
    }

    public void death()
    {
        Animator animator = player.GetComponent<Animator>();
        animator.SetTrigger("death");

    }


}
