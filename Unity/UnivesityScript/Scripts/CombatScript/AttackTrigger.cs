using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public EnemyAttackSubject enemyAttackSubject;
    [SerializeField] private GameObject activeEnemy;

    public void OnTriggerEnter(Collider other)
    {

        if (enemyAttackSubject.GetHasHit()) return;

        if (other.CompareTag("root"))
        {
            enemyAttackSubject.setHasHit(true);
            Debug.Log($"Enemy: {activeEnemy.GetComponent<Enemy>()}");

            PlayerStats player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
            player.riceviAttacco(activeEnemy.GetComponent<Enemy>());


        }
    }
}
