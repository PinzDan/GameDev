using gameSpaces.enemies;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using JetBrains.Annotations;
using UnityEngine.UI;
using UnityEngine.Rendering;

public enum EnemyState
{
    Idle,
    Combat,
    Attack1,
    Attack2,
    Dead
}




public class Enemy : MonoBehaviour
{

    // ========== Attack events ==========
    void OnEnable()
    {

        if (gameObject.tag == "activeEnemy")
        {
            EnemyAttackSubject.OnAttackStateEnter += setAttack1;
            EnemyAttackSubject.OnAttackStateExit += setCombat;

            // EnemyAttackSubject2.OnAttackStateEnter += setAttack2;
            // EnemyAttackSubject2.OnAttackStateExit += setCombat;
        }


    }

    void OnDisable()
    {

        if (gameObject.tag == "activeEnemy")
        {
            EnemyAttackSubject.OnAttackStateEnter -= setAttack1;
            EnemyAttackSubject.OnAttackStateExit -= setCombat;

            // EnemyAttackSubject2.OnAttackStateEnter -= setAttack2;
            // EnemyAttackSubject2.OnAttackStateExit -= setCombat;
        }

    }

    // ========== Enemy Data & References ==========
    public EnemyData enemyData;

    [SerializeField] private Animator animator;
    [SerializeField] private EnemyState state;
    [SerializeField] private EnemyType enemyType;

    [SerializeField] private SphereCollider attack1;
    [SerializeField] private SphereCollider attack2;
    [SerializeField] private StatusEffectData? effect;
    [SerializeField] private MeshCollider Collider;

    public PlayerStateManager PlayerStateManager;
    public Transform player;

    private GameObject enemyObject;
    private int id;
    private Quaternion originalRotation;

    private bool battle;



    // ========== Status Effects ==========

    //** 
    public Slider health;
    public Color defaultColor;

    private bool isCharging;
    private bool isDefending;


    // ========== Stats ==========
    public float CurrentAttack { get; private set; } = 0f;



    // ========== Initialization ==========
    public void Awake()
    {
        battle = false;
        isCharging = false;
        isDefending = false;

        transform.rotation = originalRotation;

        animator = GetComponent<Animator>();
        setMeshCollider();

    }

    public void Start()
    {
        PlayerStateManager = GameObject.Find("PlayerManager").GetComponent<PlayerStateManager>();
        player = GameObject.Find("RPGHeroPolyart").transform;


        health = transform.Find("enemyCanvas/Slider").GetComponent<Slider>();
        health.maxValue = enemyData.maxHealth;
        health.value = enemyData.maxHealth;

        StartCoroutine(defence());
        effect = enemyData.GetStatusEffectData();


    }

    public void Inizialize(GameObject enemyObject, int id, SphereCollider attack1, SphereCollider attack2)
    {
        this.enemyObject = enemyObject;
        this.id = id;
        this.state = EnemyState.Idle;

        this.animator = enemyObject.GetComponent<Animator>();
        this.attack1 = attack1;
        this.attack2 = attack2;
    }

    public void setEnemyData(EnemyData enemyData) => this.enemyData = enemyData;
    public void setId(int id) => this.id = id;
    public void setRotation(Quaternion rotation) => originalRotation = rotation;


    public void setEnemyType() => enemyType = enemyData.type;

    private void setMeshCollider()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("enemyCollider"))
            {
                Collider = child.GetComponent<MeshCollider>();
                break;
            }
        }
    }

    // ========== Getters ==========
    public GameObject getEnemy() => enemyObject;
    public int getId() => id;
    public float getCurrentAttack() => CurrentAttack;
    public EnemyState getState() => state;

    public EnemyType getEnemyType() => enemyType;
    public SphereCollider getAttack1() => attack1;
    public SphereCollider getAttack2() => attack2;

    // ========== Enemy State Control ==========
    public void setIdle() => state = EnemyState.Idle;
    public void setCombat()
    {
        Debug.Log("Set combat chiamato");
        state = EnemyState.Combat;
        gameObject.tag = "activeEnemy";

    }

    public void setAttack1() => state = EnemyState.Attack1;
    public void setAttack2() => state = EnemyState.Attack2;

    // ========== Unity Events ==========
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("root")) return;

        // gestione di eventuali status effect <-

        if ((state == EnemyState.Idle) && !battle)
        {
            setCombat();
            battle = true;
            PlayerStateManager.setCombat();
            setCombatAnimation();
            StartCoroutine(WaitForCombatAnimThenWalk());
            setAttack2();
        }
        else if (state == EnemyState.Attack2)
        {
            setAttack1();
        }

        StartCoroutine(attackCooldown());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("root")) return;

        if (state == EnemyState.Attack1)
        {
            setAttack2();
        }
        else if (state == EnemyState.Attack2)
        {
            resetEnemy();
            battle = false;
            setIdle();
            stopCombat();
            PlayerStateManager.Reset();

            if (animator.GetBool("Walking")) // serve a stoppare la camminate se attiva (dipende dal boolean walking)
                animator.SetBool("Walking", false);
        }
    }

    // ========== Combat Behavior ==========
    private void resetEnemy()
    {
        gameObject.tag = "enemy";
        transform.rotation = originalRotation;
        transform.Find("EnemyCanvas" + id); // icon reference (not used?)
    }

    private IEnumerator attackCooldown()
    {
        yield return new WaitForSeconds(0.5f);
    }

    public void Attack1()
    {
        if (animator == null) return;

        animator.SetInteger("canAttack", 1);
        CurrentAttack = enemyData.attackDamageBonus;
    }

    public void Attack2()
    {
        if (animator == null) return;

        animator.SetInteger("canAttack", 2);
        CurrentAttack = enemyData.attackDamageBonus;
    }

    // ========== Special Abilities ==========


    //**
    public void changeColorHealthBar(Slider healthBar)
    {

        Image fill = transform.Find("enemyCanvas/Slider/Fill Area/Fill").GetComponent<Image>();

        defaultColor = fill.color; ;
        fill.color = new Color32(0x3A, 0x6F, 0xC0, 0xFF); // #3A6FC0

    }

    public void resetColorHealthBar(Slider healthBar)
    {
        Debug.Log("Reset color health bar");
        isDefending = false;
        Image fill = transform.Find("enemyCanvas/Slider/Fill Area/Fill").GetComponent<Image>();
        Debug.Log("Default color: " + defaultColor);
        fill.color = defaultColor;
    }

    public IEnumerator defence()

    {
        Debug.Log("Defence coroutine started");
        if (enemyData.statusEffects == null || enemyData.statusEffects.Length == 0)
        {
            Debug.Log("No status effects for this enemy.");
            yield break;
        }


        if (enemyData.statusEffects[0].effectName == "defence")

        {



            EnemiesStats enemiesStats = GetComponent<EnemiesStats>();

            yield return new WaitUntil(() => enemiesStats.currentHealth <= enemiesStats.maxHealth / 2);

            isDefending = true;
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("defence");

            changeColorHealthBar(health);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Defence"));

            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Defence"));

            resetColorHealthBar(health);
        }
    }

    public IEnumerator BurningEffect()
    {
        Debug.Log("Burning effect started");
        PlayerStats stats = player.GetComponent<PlayerStats>();
        Debug.Log($"Applico l'effetto {effect?.effectName} al player");
        stats.ApplyStatusEffect(effect.effectName);
        yield return null;

    }

    // l'animazione è in loop, è lasciata appositamente duplicarsi tramite l'animation event.
    public IEnumerator chargingStatusEffect()
    {


        StatusEffectData effectData = enemyData.GetStatusEffectData();

        if (isCharging) yield break;

        if (effectData != null)
        {
            isCharging = true;
            ParticleSystem particle = Instantiate(effectData.vfxChargingPrefab, transform);

            var shape = particle.shape;
            shape.radius = 0.5F;
            particle.Play();


            Debug.Log($"Effect name: {effectData.effectName}");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(effectData.effectName));

            //yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            yield return new WaitForSeconds(9.25f); //Dall'animator ho preso il valore
            Debug.Log("Stop");
            //particle.Stop();
            //Destroy(particle.gameObject);

            particle = Instantiate(effectData.vfxExplodePrefab, transform);
            explodeStatusEffect(particle);

            PlayerStats stats = player.GetComponent<PlayerStats>();
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= attack1.radius * 3)
            {
                Debug.Log("distanza: " + distance + " " + attack1.radius * 3);
                //se il player è in un range poco superiore viene avvelenato
                stats.ApplyStatusEffect(effectData.effectName);
            }
        }
    }



    public IEnumerator explodeStatusEffect(ParticleSystem particle)
    {
        Debug.Log("Esplodo");
        if (particle != null)
        {


            particle.Play();
            yield return new WaitForSeconds(5f);
            particle.Stop();
            Destroy(particle.gameObject);
            isCharging = false;
        }
    }

    /* =========== Animation Events ========== */

    public void attackReset()
    {
        if (animator != null)
        {
            animator.SetInteger("CanAttack", 0);
        }
    }

    public void getHit(float damage)
    {


        if (isDefending)
        {
            Debug.Log("Enemy is defending, no damage taken.");
            return;
        }

        Debug.Log("Enemy get hit");

        EnemiesStats enemiesStats = gameObject.GetComponent<EnemiesStats>();
        enemiesStats.callTakeDamage(damage);
    }

    public void Randomizer()
    {
        if (animator != null)
        {
            int randomValue = UnityEngine.Random.Range(0, 6);
            animator.SetInteger("Randomizer", randomValue);
        }
    }

    // ========== Animator Logic ==========
    public void setCombatAnimation()
    {
        animator?.SetBool("Combat", true);
    }

    public void stopCombat()
    {
        animator.SetBool("Combat", false);
        animator.SetInteger("CanAttack", 0);
        animator.SetTrigger("exitCombat");
    }

    private IEnumerator WaitForCombatAnimThenWalk()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("SenseSomethingST"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("SenseSomethingST"));
    }

    public void setWalking() => animator.SetBool("Walking", true);
    public void unsetWalking() => animator.SetBool("Walking", false);

    // ========== Movement ==========
    void Update()
    {
        if (battle) chase();

        if (state == EnemyState.Attack1)
            Attack1();
        else if (state == EnemyState.Attack2)
            Attack2();

        if (animator.GetBool("Walking"))
            Walking();
    }

    void chase()
    {
        Transform enemyCanvasIcon = transform.Find("EnemyCanvas" + id); // Not used

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion playerRotation = Quaternion.LookRotation(transform.position - player.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 500f);
            player.rotation = Quaternion.Slerp(player.rotation, playerRotation, Time.deltaTime * 200f);
        }
    }

    public void startWalking()
    {
        if (player == null || animator.GetBool("Walking")) return;

        animator.SetBool("Walking", true);
    }

    public void Walking()
    {
        Transform transformPlayer = player.transform;

        float currentDistance = Vector3.Distance(transform.position, transformPlayer.position);

        Debug.Log($"CurrentDistance: {currentDistance}");
        if (currentDistance < enemyData.attack01Range)
        {
            animator.SetBool("Walking", false);
            return;
        }

        Vector3 direction = (transformPlayer.position - transform.position).normalized;
        transform.position += direction * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }


    // ========== Destruction ==========
    public void delete() => Destroy(gameObject);
}
