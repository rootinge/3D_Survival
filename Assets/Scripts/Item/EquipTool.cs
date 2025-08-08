using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate; // 공격 속도
    private bool attacking; // 공격 중인지 여부
    public float attackDistance; // 공격 거리

    [Header("Resource Gathering")]
    public bool doesGatherResources; // 자원 수집 여부

    [Header("Comdbt")]
    public bool dosesDealDamage; // 피해를 주는지 여부
    public int damage; // 공격력

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public override void OnAttackInput()
    {
        if(!attacking)
        {
            attacking = true;
            animator.SetTrigger("Attack");
            Invoke("OnCanAttack", attackRate);
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }
}
