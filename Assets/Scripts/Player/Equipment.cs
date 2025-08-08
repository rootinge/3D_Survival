using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip; // 현재 장착된 장비
    public Transform equipParent; // 장비를 장착할 부모 객체 트렌스폼

    private PlayerController controller; // 플레이어 컨트롤러 참조
    private PlayerCondition condition; // 플레이어 상태 참조

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
    }

    public void EquipNew(ItemData data)
    {
        UnEquip();
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>(); 
    }

    public void UnEquip()
    {
        if(curEquip!= null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput(); // 현재 장착된 장비의 공격 입력 처리
        }
    }
}
