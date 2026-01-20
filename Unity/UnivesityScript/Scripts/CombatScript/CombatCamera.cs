using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CombatCamera : MonoBehaviour
{
    public void OnEnable()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("activeEnemy");
    }
}