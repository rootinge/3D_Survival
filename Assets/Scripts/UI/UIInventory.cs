using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    public ItemSlot[] slots; // ������ ���� �迭

    public GameObject inventoryWindow; // �κ��丮 â ������Ʈ
    public Transform slotPanel; // ���� �г� Ʈ������
    public Transform dropPosition; // ��� ��ġ Ʈ������


    [Header("Select Item")]
    public TextMeshProUGUI selectedItemName; // ���õ� ������ �̸�
    public TextMeshProUGUI selectedItemDescription; // ���õ� ������ ����
    public TextMeshProUGUI selectedStatName; // ���õ� ������ ���� �̸�
    public TextMeshProUGUI selectedStatValue; // ���õ� ������ ���� ��
    public GameObject useButton; // ��� ��ư
    public GameObject equipButton; // ���� ��ư
    public GameObject unequipeButton; // ���� ���� ��ư
    public GameObject dropButton; // ��� ��ư

    private PlayerController controller; // �÷��̾� ��Ʈ�ѷ�
    private PlayerCondition condition; // �÷��̾� ����

    ItemData selectedItem; // ���õ� ������ ������
    int selectedItemIndex = 0; // ���õ� ������ �ε���

    int curEquipIndex; // ���� ������ ������ �ε���

    // Start is called before the first frame update
    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
        dropPosition = CharacterManager.Instance.Player.dropPosition;

        controller.inventory += Toggle; // �κ��丮 ��� ��������Ʈ ���
        CharacterManager.Instance.Player.addItem += AddItem; // ������ �߰� ��������Ʈ ���

        inventoryWindow.SetActive(false); // �κ��丮 â ��Ȱ��ȭ
        slots = new ItemSlot[slotPanel.childCount]; // ���� �迭 �ʱ�ȭ

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotPanel.GetChild(i).GetComponent<ItemSlot>(); // ���� ������Ʈ ��������
            slots[i].index = i; // ���� �ε��� ����
            slots[i].inventory = this; // ������ �κ��丮 ����
        }

        ClearSelctedItemWindow(); // ���õ� ������ â �ʱ�ȭ
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClearSelctedItemWindow()
    {
        selectedItemName.text = string.Empty; // ���õ� ������ �̸� �ʱ�ȭ
        selectedItemDescription.text = string.Empty; // ���õ� ������ ���� �ʱ�ȭ
        selectedStatName.text = string.Empty; // ���õ� ������ ���� �̸� �ʱ�ȭ
        selectedStatValue.text = string.Empty; // ���õ� ������ ���� �� �ʱ�ȭ
       
        useButton.SetActive(false); // ��� ��ư ��Ȱ��ȭ
        equipButton.SetActive(false); // ���� ��ư ��Ȱ��ȭ
        unequipeButton.SetActive(false); // ���� ���� ��ư ��Ȱ��ȭ
        dropButton.SetActive(false); // ��� ��ư ��Ȱ��ȭ
    }

    public void Toggle()
    {
        if(IsOpen())
        {
            inventoryWindow.SetActive(false); // �κ��丮 â �ݱ�
            //controller.enabled = true; // �÷��̾� ��Ʈ�ѷ� Ȱ��ȭ
        }
        else
        {
            inventoryWindow.SetActive(true); // �κ��丮 â ����
            //controller.enabled = false; // �÷��̾� ��Ʈ�ѷ� ��Ȱ��ȭ
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy; // �κ��丮 â�� �����ִ��� ���� ��ȯ
    }

    void AddItem()
    {
        ItemData data = CharacterManager.Instance.Player.itemData; // �÷��̾��� ������ ������ ��������

        if(data.canStack)
        {
            ItemSlot slot = GetItemStack(data); // ������ ���� ������ ���� ã��
            if (slot != null)
            {
                slot.quantity++; // ������ ���� ����
                UpdateUI(); 
                CharacterManager.Instance.Player.itemData = null; // �÷��̾��� ������ ������ �ʱ�ȭ
                return;
            }

        }

        ItemSlot emptySlot = GetEmptySlot(); // �� ���� ã��

        if (emptySlot != null)
        {
            emptySlot.item = data; // �� ���Կ� ������ ������ ����
            emptySlot.quantity = 1; // ���� 1�� ����
            UpdateUI();
            CharacterManager.Instance.Player.itemData = null; // �÷��̾��� ������ ������ �ʱ�ȭ
            return;
        }

        ThrowItem(data);
        CharacterManager.Instance.Player.itemData = null; // �÷��̾��� ������ ������ �ʱ�ȭ

    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                slots[i].Set(); // ���� ����
            }
            else
            {
                slots[i].Clear(); // ���� �ʱ�ȭ
            }
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i]; // ���� ������ ���� ��ȯ
            }
        }
        return null; // ���� ������ ������ ������ null ��ȯ
    }

    ItemSlot GetEmptySlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                return slots[i]; // �� ���� ��ȯ
            }
        }
        return null; // �� ������ ������ null ��ȯ
    }

    
    void ThrowItem(ItemData data) 
    {
        Instantiate(data.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * Random.value * 360)); // ������ ��� ������ �ν��Ͻ�ȭ
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return; // �������� ������ ��ȯ

        selectedItem = slots[index].item; // ���õ� ������ ������ ����
        selectedItemIndex = index; // ���õ� ������ �ε��� ����

        selectedItemName.text = selectedItem.displayName; // ���õ� ������ �̸� ����
        selectedItemDescription.text = selectedItem.description; // ���õ� ������ ���� ����

        selectedStatName.text = string.Empty; // ���õ� ������ ���� �̸� �ʱ�ȭ
        selectedStatValue.text = string.Empty; // ���õ� ������ ���� �� �ʱ�ȭ

        
        for (int i = 0; i < selectedItem.consumbales.Length; i++)
        {
            selectedStatName.text += selectedItem.consumbales[i].type.ToString() + "\n";    
            selectedStatValue.text += selectedItem.consumbales[i].value.ToString() + "\n"; 
        }

        useButton.SetActive(selectedItem.Type == ItemType.Consumable); // �Һ� �������̸� ��� ��ư Ȱ��ȭ
        equipButton.SetActive(selectedItem.Type == ItemType.Equipable && !slots[index].equipped); // ���� ������ �������̸� ���� ��ư Ȱ��ȭ
        unequipeButton.SetActive(selectedItem.Type == ItemType.Equipable && slots[index].equipped); // ������ �������̸� ���� ���� ��ư Ȱ��ȭ
        dropButton.SetActive(true); // ��� ��ư Ȱ��ȭ
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
                        condition.Heal(selectedItem.consumbales[i].value); // ü�� ȸ��
                        break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.consumbales[i].value); // ��� ȸ��
                        break;
                }

            }
            RemoveSelectedItem(); // ���õ� ������ ����
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem); // ������ ���
        RemoveSelectedItem(); // ���õ� ������ ����
    }

    void RemoveSelectedItem()
    {
        slots[selectedItemIndex].quantity--; // ���õ� ������ ���� ����

        if (slots[selectedItemIndex].quantity <= 0)
        {
            selectedItem = null; // ���õ� ������ �ʱ�ȭ
            slots[selectedItemIndex].item = null; // ������ ������ ������ �ʱ�ȭ
            selectedItemIndex = -1; // ���õ� ������ �ε��� �ʱ�ȭ
            ClearSelctedItemWindow(); // ���õ� ������ â �ʱ�ȭ
        }

        UpdateUI(); // UI ������Ʈ
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            UnEquip(curEquipIndex); // ���� ������ ������ ����
        }

        slots[selectedItemIndex].equipped = true; // ������ ���� ���� ����
        curEquipIndex = selectedItemIndex; // ���� ������ ������ �ε��� ����
        CharacterManager.Instance.Player.equip.EquipNew(selectedItem); // �÷��̾��� ��� ������Ʈ�� ������ ����
        UpdateUI(); // UI ������Ʈ

        SelectItem(selectedItemIndex); // ���õ� ������ ������Ʈ
    }

    void UnEquip(int index)
    {
        slots[index].equipped = false; // ������ ���� ���� ����
        CharacterManager.Instance.Player.equip.UnEquip(); // �÷��̾��� ��� ������Ʈ���� ������ ���� ����
        UpdateUI(); // UI ������Ʈ

        // ���õ� �������� ���� �ε����� ������ ���õ� ������ ������Ʈ
        if (selectedItemIndex == index)
        {
            SelectItem(selectedItemIndex); // ���õ� ������ ������Ʈ
        }
    }

    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex); // ���õ� ������ �ε����� ������ ���� ����
    }
}
