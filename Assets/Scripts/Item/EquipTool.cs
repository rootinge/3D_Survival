using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate; // ���� �ӵ�
    private bool attacking; // ���� ������ ����
    public float attackDistance; // ���� �Ÿ�

    [Header("Resource Gathering")]
    public bool doesGatherResources; // �ڿ� ���� ����

    [Header("Comdbt")]
    public bool dosesDealDamage; // ���ظ� �ִ��� ����
    public int damage; // ���ݷ�

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
