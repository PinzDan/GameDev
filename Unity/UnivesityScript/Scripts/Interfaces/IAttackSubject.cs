using UnityEngine;

public interface IAttackSubject
{
    Collider GetCollider();
    void OnAttackEnter();
    void OnAttackExit();
}