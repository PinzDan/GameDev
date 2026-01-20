using System;
using System.Collections.Generic;
using UnityEngine;
using gameSpaces.enemies;
using UnityEngine.Rendering;


public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    private Dictionary<int, Enemy> enemies = new Dictionary<int, Enemy>();


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);


    }
    // Start is called before the first frame update
    void OnEnable()
    {
        addEnemy();
    }


    public void addEnemy()
    {
        int id = 0;
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("enemy");


        foreach (GameObject enemyObject in enemyObjects)
        {

            enemies.Add(id, setEnemy(enemyObject, id++));
        }
    }

    public Enemy setEnemy(GameObject enemy, int id)
    {
        if (enemy == null) return null;


        string enemyName = enemy.name;
        EnemyData data = null;


        if (Enum.TryParse(enemyName, true, out EnemyType parsedType))

        {

            data = Resources.Load<EnemyData>(EnemyDatabase.DataPaths[parsedType]);
            Debug.Log("Loaded data: " + data + " type: " + data?.GetType());


        }



        // Aggiunt Collider e stats 

        SphereCollider attack1 = enemy.AddComponent<SphereCollider>();//un collider per i tipi di attacchi standard

        attack1.radius = data.attack01Range;
        attack1.center = new Vector3(0, 0, 0);
        attack1.isTrigger = true;


        SphereCollider attack2 = enemy.AddComponent<SphereCollider>();

        attack2.radius = data.attack02Range;
        attack2.center = new Vector3(0, 0, 0);
        attack2.isTrigger = true;

        // Componenti principali
        EnemiesStats stats = enemy.AddComponent<EnemiesStats>();
        stats.maxHealth = data.maxHealth;
        stats.currentHealth = data.maxHealth;

        stats.rewardPoints = data.rewardPoints; // assegna i punti alla morte
        Enemy newEnemy = enemy.AddComponent<Enemy>();
        newEnemy.Inizialize(enemy, id, attack1, attack2);
        //newEnemy.setEffect(effectData); 
        newEnemy.setEnemyData(data); // <- assegnazione data
        newEnemy.setEnemyType();
        newEnemy.setRotation(enemy.transform.rotation);

        // Altri componenti

        enemy.AddComponent<Events>();
        enemy.AddComponent<EnemySounds>();
        enemy.AddComponent<enemyStatsPosition>();


        return (newEnemy);
    }


    public void removeDeadEnemies(int id)
    {
        if (enemies.ContainsKey(id))
        {
            enemies[id].delete();
            enemies.Remove(id);

        }
    }


    public EnemyType getType(string name)

    {
        if (Enum.TryParse(name, true, out EnemyType parsedType))
        {

            return parsedType;
        }

        return EnemyType.Slime;
    }
}

