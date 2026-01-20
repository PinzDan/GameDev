using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abilitaSpada : StateMachineBehaviour
{

    [SerializeField] private MeshCollider weaponCollider;

    public static System.Action OnAttackStateEnter;
    public static System.Action OnAttackStateExit;

    private GameObject weapon;


    void Awake()
    {
        weapon = GameObject.FindGameObjectWithTag("weapon");
        weaponCollider = weapon.GetComponent<MeshCollider>();
        weaponCollider.enabled = false;
    }
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        weaponCollider.enabled = true;

        OnAttackStateEnter?.Invoke();
    }



    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weaponCollider.enabled = false;
        OnAttackStateExit?.Invoke();
    }


}
