using System.Runtime.InteropServices.WindowsRuntime;
using gameSpaces.enemies;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyType type;
    public float attack01Range;
    public float attack02Range;
    public int maxHealth;
    public float speed;
    public float attackDamageBase;
    public float attackDamageBonus;

    public StatusEffectData[] statusEffects; // Array di effetti di stato applicabili all'enemy

    public int rewardPoints;
    public GameObject prefab;

    // Al momento è semplificato. normalmente un nemico può applicare 0, 1 o più effetti.
    public StatusEffectData GetStatusEffectData() => statusEffects[0];
    //public float attackCooldown;

    // Add any other enemy-specific data here
}