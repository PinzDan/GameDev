using System.Collections;
using UnityEngine;
public class PlayerAttackSubject : MonoBehaviour, IAttackSubject
{
    public static event System.Action OnAttackStateEnter;
    public static event System.Action OnAttackStateExit;

    [SerializeField] private MeshCollider weaponCollider;

    private bool hasHit;

    public float dannoBase = 10f;         // danno base
    private float dannoMoltiplicatore = 1f; // moltiplicatore attacco normale/s
    private void Awake()
    {

        weaponCollider.enabled = true;
        weaponCollider.isTrigger = false; // Ensure the collider is not a trigger initially
    }

    public void OnAttackEnter()
    {
        dannoBase = GameManager.Instance.getDannoBase();
        dannoMoltiplicatore = 1f;
        hasHit = false;
        Debug.Log("PlayerAttackSubject: OnAttackEnter called");
        weaponCollider.isTrigger = true; // Enable trigger for attack
        OnAttackStateEnter?.Invoke();
    }


    public void OnAttackExit()
    {
        weaponCollider.isTrigger = false;
        OnAttackStateExit?.Invoke();
    }

    public Collider GetCollider() => weaponCollider;
    public bool GetHasHit() => hasHit;
    public float GetDanno()
    {
        return dannoBase * dannoMoltiplicatore;
    }
    public void setHasHit(bool hasHit)
    {
        this.hasHit = hasHit;
    }

}
