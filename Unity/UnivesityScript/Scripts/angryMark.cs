using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angryMark : StateMachineBehaviour
{
    [SerializeField] private GameObject markerPrefab;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (markerPrefab == null)
        {
            Transform markerTransform = animator.transform.Find("angryMark");
            if (markerTransform != null)
                markerPrefab = markerTransform.gameObject;
        }


        markerPrefab.SetActive(true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        markerPrefab.SetActive(false);
    }
}
