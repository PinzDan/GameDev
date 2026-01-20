using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class EnemiesStats : CharacterStats
{
    public static event Action<int> addScore; // evento con punteggio
    [SerializeField] private GameObject enemyHP;
    [SerializeField] private Slider Hpslider;

    private float previousHealth;
    private ParticleSystem particle;

    private PlayerStateManager playerState;

    public int rewardPoints; // punti dati alla morte del nemico


    new
        // Start is called before the first frame update
        void Start()
    {

        base.Start();

        playerState = GameObject.Find("PlayerManager").GetComponent<PlayerStateManager>();
        particle = gameObject.GetComponent<ParticleSystem>();
        Debug.Log("Particle: " + particle + " for enemy: " + gameObject.name + "get parent: " + gameObject.transform.parent);
        // GameObject canvas = GameObject.Find("EnemyCanvas" + gameObject.GetComponent<Enemy>().getId());

        Debug.Log("Max health: " + maxHealth + " for enemy: " + gameObject.name);
        GameObject canvas = transform.Find("enemyCanvas")?.gameObject;

        // enemyHP = canvas.transform.Find("enemyHP").gameObject;
        Hpslider = canvas.transform.Find("Slider").GetComponent<Slider>();
        // Debug.Log("Slider: " + Hpslider);
        // Debug.Log("Hp enemy: " + Hpslider);
        // if (enemyHP != null)
        // {

        //Debug.Log("Slider: " + Hpslider);
        if (Hpslider != null)
        {
            Hpslider.maxValue = maxHealth;
            Hpslider.value = currentHealth;
        }


        // 
    }




    public IEnumerator animationDeath()
    {
        if (currentHealth <= 0)
        {

            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Dead");

            //Aspetta che l'animazione "Dead" inizi
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));

            StartCoroutine(waitParticle());
        }
    }

    private IEnumerator waitParticle()
    {
        Debug.Log("Waiting for particle: " + particle + " for enemy: " + gameObject.name);
        particle.Play();
        playerState.Reset();
        //gameObject.tag = "Untagged";

        yield return new WaitForSeconds(particle.main.duration);

        die();
    }

    public void updateHpSlider()
    {
        if (currentHealth != previousHealth)
        {
            Hpslider.value = currentHealth;
            previousHealth = currentHealth;
            // StartCoroutine(checkDeath());
        }
    }


    public void callTakeDamage(float danno = 10)
    {
        takeDamage(danno);
    }
    protected override void takeDamage(float danno = 10)
    {
        Debug.Log("Enemy " + gameObject.name + " taking damage: " + danno);
        Animator animator = gameObject.GetComponent<Animator>();
        base.takeDamage(danno);
        updateHpSlider();

        if (currentHealth <= 0)
            StartCoroutine(animationDeath());
        else
            animator.SetTrigger("getHit");
    }
    public void subtractHp()
    {
        this.takeDamage(20F); //da modificare con i data del player
        updateHpSlider();
    }

    protected override void die()
    {
        base.die();
        addScore?.Invoke(rewardPoints);
        Destroy(gameObject);
    }
}
