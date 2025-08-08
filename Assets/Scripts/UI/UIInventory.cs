using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots; // 아이템 슬롯 배열

    public GameObject inventoryWindow; // 인벤토리 창 오브젝트
    public Transform slotPanel; // 슬롯 패널 트랜스폼
    public Transform dropPosition; // 드롭 위치 트랜스폼


    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName; // 선택된 아이템 이름
    public TextMeshProUGUI selectedItemDescription; // 선택된 아이템 설명
    public TextMeshProUGUI selectedStatName; // 선택된 아이템 스탯 이름
    public TextMeshProUGUI selectedStatValue; // 선택된 아이템 스탯 값
    public GameObject useButton; // 사용 버튼
    public GameObject equipButton; // 장착 버튼
    public GameObject unequipeButton; // 장착 해제 버튼
    public GameObject dropButton; // 드롭 버튼

    private PlayerController controller; // 플레이어 컨트롤러
    private PlayerCondition condition; // 플레이어 상태

    ItemData selectedItem; // 선택된 아이템 데이터
    int selectedItemIndex = 0; // 선택된 아이템 인덱스

    int curEquipIndex; // 현재 장착된 아이템 인덱스

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory += Toggle; // 인벤토리 토글 델리게이트 등록
        CharacterManager.Instance.Player.addItem += AddItem; // 아이템 추가 델리게이트 등록

        inventoryWindow.SetActive(false); // 인벤토리 창 비활성화
        slots = new ItemSlot[slotPanel.childCount]; // 슬롯 배열 초기화

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>(); // 슬롯 컴포넌트 가져오기
            slots[i].index = i; // 슬롯 인덱스 설정
            slots[i].inventory = this; // 슬롯의 인벤토리 설정
        }

        ClearSelctedItemWindow(); // 선택된 아이템 창 초기화
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSelctedItemWindow()
    {
        selectedItemName.text = string.Empty; // 선택된 아이템 이름 초기화
        selectedItemDescription.text = string.Empty; // 선택된 아이템 설명 초기화
        selectedStatName.text = string.Empty; // 선택된 아이템 스탯 이름 초기화
        selectedStatValue.text = string.Empty; // 선택된 아이템 스탯 값 초기화
       
        useButton.SetActive(false); // 사용 버튼 비활성화
        equipButton.SetActive(false); // 장착 버튼 비활성화
        unequipeButton.SetActive(false); // 장착 해제 버튼 비활성화
        dropButton.SetActive(false); // 드롭 버튼 비활성화
    }

    public void Toggle()
    {
        if(IsOpen())
        {
            inventoryWindow.SetActive(false); // 인벤토리 창 닫기
            //controller.enabled = true; // 플레이어 컨트롤러 활성화
        }
        else
        {
            inventoryWindow.SetActive(true); // 인벤토리 창 열기
            //controller.enabled = false; // 플레이어 컨트롤러 비활성화
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy; // 인벤토리 창이 열려있는지 여부 반환
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData; // 플레이어의 아이템 데이터 가져오기

        if(data.canStack)
        {
            ItemSlot slot = GetItemStack(data); // 아이템 스택 가능한 슬롯 찾기
            if (slot != null)
            {
                slot.quantity++; // 슬롯의 수량 증가
                UpdateUI(); 
                CharacterManager.Instance.Player.itemData = null; // 플레이어의 아이템 데이터 초기화
                return;
            }

        }

        ItemSlot emptySlot = GetEmptySlot(); // 빈 슬롯 찾기

        if (emptySlot != null)
        {
            emptySlot.item = data; // 빈 슬롯에 아이템 데이터 설정
            emptySlot.quantity = 1; // 수량 1로 설정
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null; // 플레이어의 아이템 데이터 초기화
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null; // 플레이어의 아이템 데이터 초기화

    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set(); // 슬롯 설정
            }
            else
            {
                slots[i].Clear(); // 슬롯 초기화
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i]; // 스택 가능한 슬롯 반환
            }
        }
        return null; // 스택 가능한 슬롯이 없으면 null 반환
    }

    ItemSlot GetEmptySlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i]; // 빈 슬롯 반환
            }
        }
        return null; // 빈 슬롯이 없으면 null 반환
    }

    
    void ThrowItem(ItemData data) 
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360)); // 아이템 드롭 프리팹 인스턴스화
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return; // 아이템이 없으면 반환

        selectedItem = slots[index].item; // 선택된 아이템 데이터 설정
        selectedItemIndex = index; // 선택된 아이템 인덱스 설정

        selectedItemName.text = selectedItem.displayName; // 선택된 아이템 이름 설정
        selectedItemDescription.text = selectedItem.description; // 선택된 아이템 설명 설정

        selectedStatName.text = string.Empty; // 선택된 아이템 스탯 이름 초기화
        selectedStatValue.text = string.Empty; // 선택된 아이템 스탯 값 초기화

        
        for (int i = 0; i < selectedItem.consumbales.Length; i++)
        {
            selectedStatName.text += selectedItem.consumbales[i].type.ToString() + "\n";    
            selectedStatValue.text += selectedItem.consumbales[i].value.ToString() + "\n"; 
        }

        useButton.SetActive(selectedItem.Type == ItemType.Consumable); // 소비 아이템이면 사용 버튼 활성화
        equipButton.SetActive(selectedItem.Type == ItemType.Equipable && !slots[index].equipped); // 장착 가능한 아이템이면 장착 버튼 활성화
        unequipeButton.SetActive(selectedItem.Type == ItemType.Equipable && slots[index].equipped); // 장착된 아이템이면 장착 해제 버튼 활성화
        dropButton.SetActive(true); // 드롭 버튼 활성화
    }

    public void OnUseButton()
    {
        if(selectedItem.Type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.consumbales.Length; i++)
            {
                switch (selectedItem.consumbales[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.consumbales[i].value); // 체력 회복
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumbales[i].value); // 허기 회복
                        break;
                }

            }
            RemoveSelectedItem(); // 선택된 아이템 제거
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem); // 아이템 드롭
        RemoveSelectedItem(); // 선택된 아이템 제거
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--; // 선택된 아이템 수량 감소

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null; // 선택된 아이템 초기화
            slots[selectedItemIndex].item = null; // 슬롯의 아이템 데이터 초기화
            selectedItemIndex = -1; // 선택된 아이템 인덱스 초기화
            ClearSelctedItemWindow(); // 선택된 아이템 창 초기화
        }

        UpdateUI(); // UI 업데이트
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex); // 현재 장착된 아이템 해제
        }

        slots[selectedItemIndex].equipped = true; // 슬롯의 장착 상태 설정
        curEquipIndex = selectedItemIndex; // 현재 장착된 아이템 인덱스 설정
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem); // 플레이어의 장비 컴포넌트에 아이템 장착
        UpdateUI(); // UI 업데이트

        SelectItem(selectedItemIndex); // 선택된 아이템 업데이트
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false; // 슬롯의 장착 상태 해제
        CharacterManager.Instance.Player.equip.UnEquip(); // 플레이어의 장비 컴포넌트에서 아이템 장착 해제
        UpdateUI(); // UI 업데이트

        // 선택된 아이템이 현재 인덱스와 같으면 선택된 아이템 업데이트
        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex); // 선택된 아이템 업데이트
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex); // 선택된 아이템 인덱스의 아이템 장착 해제
    }
}
