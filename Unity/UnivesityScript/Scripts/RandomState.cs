using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomState : StateMachineBehaviour
{
    public string parameterName = "Randomizer"; // Nome del parametro intero nell'Animator
    public int minValue = 0;
    public int maxValue = 5;

    public int random = -1;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        do
        {
            random = Random.Range(minValue, maxValue + 1); // esclude max
            Debug.Log("RandomState: Generating random value for " + random);
        } while (animator.GetInteger(parameterName) == random);

        animator.SetInteger(parameterName, random);
    }
}
