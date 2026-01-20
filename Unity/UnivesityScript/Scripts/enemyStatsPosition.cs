using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyStatsPosition : MonoBehaviour
{


    private GameObject enemyHP;
    private Transform player;
    private GameObject cloned;


    void Awake()
    {

        enemyHP = transform.Find("enemyCanvas").gameObject;
        player = GameObject.Find("RPGHeroPolyart").transform;
        // cloned = Instantiate(enemyHP);

        // cloned.name = "EnemyCanvas" + enemy.getId();
        // if (cloned != null)
        // {
        //     cloned.transform.SetParent(transform);
        //     cloned.transform.localPosition = Vector3.zero;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHP != null)
            enemyHP.transform.LookAt(player);
    }
}
