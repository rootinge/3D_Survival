using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public ItemData item; // 아이템 데이터

    public Button button; // 아이템 슬롯 버튼
    public Image icon; // 아이템 아이콘 이미지
    public TextMeshProUGUI quantityText; // 아이템 수량 텍스트
    private Outline outline; // 아이템 슬롯 아웃라인

    public UIInventory inventory; // 아이템이 속한 인벤토리

    public int index; // 아이템 슬롯의 인덱스
    public bool equipped; // 아이템이 장착되었는지 여부
    public int quantity; // 아이템의 수량

    // Start is called before the first frame update
    void Awake()
    {
        outline = GetComponent<Outline>(); // 아이템 슬롯 아웃라인 컴포넌트 가져오기
    }

    private void OnEnable()
    {
        outline.enabled = equipped; // 아이템이 장착되었으면 아웃라인 활성화
    }

    public void Set()
    {
        icon.sprite = item.icon; // 아이템 아이콘 설정
        icon.gameObject.SetActive(true); 
        quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty; // 아이템 수량 텍스트 설정

        if(outline != null)
        {
            outline.enabled = equipped; // 아이템이 장착되었으면 아웃라인 활성화
        }
    }

    public void Clear()
    {
        item = null; // 아이템 데이터 초기화
        icon.gameObject.SetActive(false); // 아이템 아이콘 비활성화
        quantityText.text = string.Empty; // 아이템 수량 텍스트 초기화
    }

    public void OnClickButton()
    {
        inventory.SelectItem(index); // 인벤토리에서 아이템 선택
    }
}
