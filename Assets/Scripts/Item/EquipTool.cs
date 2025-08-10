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
    private Camera camera;

    void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main; // ���� ī�޶� ������
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

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // ȭ�� �߾ӿ��� Ray ����

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, attackDistance))
        {
            if(doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                // �ڿ� ����
                resource.Gather(hit.point, hit.normal);
            }
        }
    }
}
