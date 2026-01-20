using UnityEngine;
using UnityEngine.Rendering;
public class EnemyAttackSubject : MonoBehaviour, IAttackSubject
{
    public static event System.Action OnAttackStateEnter;
    public static event System.Action OnAttackStateExit;

    [SerializeField] private Collider attackCollider;

    private bool hasHit; // flag per far si che venga portato a compimento l'evento per una sola collisione



    private void Awake()
    {

        attackCollider.enabled = true;
        attackCollider.isTrigger = false;
    }

    public void OnAttackEnter()
    {
        Debug.Log($"Chiamato. {hasHit}, {attackCollider} , {attackCollider.isTrigger}");
        hasHit = false;
        attackCollider.isTrigger = true;

        OnAttackStateEnter?.Invoke();
    }

    public void OnAttackExit()
    {

        attackCollider.isTrigger = false;
        OnAttackStateExit?.Invoke();
    }

    public Collider GetCollider() => attackCollider;

    public bool GetHasHit() => hasHit;
    public void setHasHit(bool hasHit)
    {
        this.hasHit = hasHit;
    }
}
