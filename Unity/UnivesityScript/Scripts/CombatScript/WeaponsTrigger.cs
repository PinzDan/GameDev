using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TypeAttack
{
    Normal,
    Special,
    none
}
public class WeaponsTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator animator;
    private EnemiesStats myenemystats;
    public PlayerAttackSubject playerAttackSubject;

    public float dannoBase = 0f;


    void OnTriggerEnter(Collider other)
    {



        if (playerAttackSubject.GetHasHit()) return;

        if (other.CompareTag("enemyDefenceCollider"))
        {
            Debug.Log("Colpito un nemico");
            playerAttackSubject.setHasHit(true);
            // Ho scelta questa soluzione, perché la logica del gioco è semplice, quindi ho preferito non usare un evento
            // Enemy enemyScript = other.GetComponentInParent<Enemy>();

            // if (enemyScript != null)
            //     enemyScript.getHit();


            GameObject enemy = GameObject.FindGameObjectWithTag("activeEnemy");

            enemy.GetComponent<Enemy>().getHit(dannoBase);
            // myenemystats = enemy.GetComponent<EnemiesStats>();
            // myenemystats.subtractHp();
        }
    }

}
