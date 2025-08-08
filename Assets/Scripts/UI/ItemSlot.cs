using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item; // ������ ������

    public Button button; // ������ ���� ��ư
    public Image icon; // ������ ������ �̹���
    public TextMeshProUGUI quantityText; // ������ ���� �ؽ�Ʈ
    private Outline outline; // ������ ���� �ƿ�����

    public UIInventory inventory; // �������� ���� �κ��丮

    public int index; // ������ ������ �ε���
    public bool equipped; // �������� �����Ǿ����� ����
    public int quantity; // �������� ����

    // Start is called before the first frame update
    void Awake()
    {
        outline = GetComponent<Outline>(); // ������ ���� �ƿ����� ������Ʈ ��������
    }

    private void OnEnable()
    {
        outline.enabled = equipped; // �������� �����Ǿ����� �ƿ����� Ȱ��ȭ
    }

    public void Set()
    {
        icon.sprite = item.icon; // ������ ������ ����
        icon.gameObject.SetActive(true); 
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty; // ������ ���� �ؽ�Ʈ ����

        if(outline != null)
        {
            outline.enabled = equipped; // �������� �����Ǿ����� �ƿ����� Ȱ��ȭ
        }
    }

    public void Clear()
    {
        item = null; // ������ ������ �ʱ�ȭ
        icon.gameObject.SetActive(false); // ������ ������ ��Ȱ��ȭ
        quantityText.text = string.Empty; // ������ ���� �ؽ�Ʈ �ʱ�ȭ
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index); // �κ��丮���� ������ ����
    }
}
