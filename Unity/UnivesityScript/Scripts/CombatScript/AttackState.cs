using Unity.VisualScripting;
using UnityEngine;

public class AttackState : StateMachineBehaviour
{
    private IAttackSubject attackSubject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        attackSubject = animator.GetComponent<IAttackSubject>();


        if (attackSubject != null)
        {
            Debug.Log("AttackState OnStateEnter called. attackSubject: " + attackSubject.GetCollider());
            attackSubject.OnAttackEnter();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (attackSubject != null)
        {
            attackSubject.OnAttackExit();
        }
    }

}