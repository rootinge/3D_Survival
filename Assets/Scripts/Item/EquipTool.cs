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
    private Camera camera;

    void Start()
    {
        animator = GetComponent<Animator>();
        camera = Camera.main; // 메인 카메라를 가져옴
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
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // 화면 중앙에서 Ray 생성

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, attackDistance))
        {
            if(doesGatherResources && hit.collider.TryGetComponent(out Resource resource))
            {
                // 자원 수집
                resource.Gather(hit.point, hit.normal);
            }
        }
    }
}
