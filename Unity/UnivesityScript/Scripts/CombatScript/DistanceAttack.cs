using System.Collections;
using UnityEngine;

public class DistanceAttack : MonoBehaviour
{
    private Vector3 moveDirection;
    private bool isMoving = true;
    private Animator enemyAnimator;

    public Transform enemyHand; // Mano del nemico
    public ParticleSystem distanceParticleSystem; // Sistema particelle

    public ParticleSystem particleSystem2;
    public SphereCollider rangeOfAttack; // Collider della roccia    
    public Enemy enemyScript; // Riferimento al nemico che lancia

    void OnEnable()
    {
        // Reset dello stato di attacco
        var attackTrigger = GetComponent<AttackTrigger>();
        if (attackTrigger != null && attackTrigger.enemyAttackSubject != null)
        {
            attackTrigger.enemyAttackSubject.setHasHit(false);
        }


        // Stacca la roccia dalla mano ma conserva posizione globale
        transform.position = enemyHand.position;
        transform.rotation = enemyHand.rotation;
        transform.parent = null;


        // Calcola la direzione verso il giocatore UNA VOLTA
        GameObject player = GameObject.FindGameObjectWithTag("foot");
        if (player != null)
        {
            moveDirection = (player.transform.position - transform.position).normalized;
        }
        else
        {
            Debug.LogWarning("Player (foot) non trovato!");
            moveDirection = Vector3.forward; // fallback
        }

        // Ottieni Animator del nemico
        GameObject enemy = GameObject.FindGameObjectWithTag("activeEnemy");
        enemyScript = enemy.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemyAnimator = enemy.GetComponent<Animator>();
        }

        isMoving = true; // abilita il movimento


    }

    void Start()
    {
        rangeOfAttack = GameObject.FindWithTag("activeEnemy").GetComponent<Enemy>().getAttack2();
    }
    void Update()
    {
        if (!isMoving) return;
        transform.position += moveDirection * Time.deltaTime * 8.0f; // velocità regolabile
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMoving) return;

        if (other.CompareTag("Player"))

        {
            Debug.Log("Collisione con terreno o root: " + other.name);
            isMoving = false; // Ferma il movimento
            gameObject.GetComponent<AudioSource>().Play();
            StartCoroutine(HandleImpact());
        }

    }

    void OnTriggerExit(Collider other)
    {

        if (other == rangeOfAttack)
        {

            Debug.Log("Uscito dal raggio di attacco: " + other.name);
            isMoving = false; // Ferma il movimento
            StartCoroutine(HandleImpact());
        }
    }

    private IEnumerator HandleImpact()
    {

        Debug.Log($"distanceParticleSystem: {distanceParticleSystem.name}, script: {enemyScript}");
        switch (distanceParticleSystem.name)
        {
            case "Fire":
                StartCoroutine(enemyScript.BurningEffect());
                Debug.Log("Effetto fuoco applicato");
                yield return spargiFuoco();

                break;
            case "RockImpact":
                Debug.Log("Impatto roccia");
                yield return rockSmash();
                break;
        }


        // Reset
        transform.SetParent(enemyHand); // torna alla mano
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(false); // nasconde la roccia
    }


    private IEnumerator rockSmash()
    {
        if (distanceParticleSystem != null)
        {
            //gameObject.GetComponent<AudioSource>().Play(); // Riproduce il suono dell'impatto
            distanceParticleSystem.Play(); // Avvia le particelle
            particleSystem2.Play();
            Debug.Log("Particelle avviate");
            yield return new WaitForSeconds(particleSystem2.main.duration);
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("smashRock");
        }
    }
    private IEnumerator spargiFuoco()
    {
        // Numero di particelle da spargere
        int numParticles = 5;
        float spreadRadius = 2.0f;

        // Trova il player
        GameObject player = GameObject.FindGameObjectWithTag("foot");
        Vector3 playerPos = player != null ? player.transform.position : transform.position;

        // Spargi i sistemi di particelle attorno al player
        for (int i = 0; i < numParticles; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * spreadRadius;
            randomOffset.y = Mathf.Abs(randomOffset.y); // opzionale: solo sopra il terreno

            Vector3 spawnPos = playerPos + randomOffset;

            // Istanzia una copia temporanea del sistema di particelle
            ParticleSystem ps = Instantiate(distanceParticleSystem, spawnPos, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }

        yield return null;
    }
}


