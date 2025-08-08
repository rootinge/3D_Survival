using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt(); // 상호작용 프롬프트를 반환하는 메서드
    public void OnInteract(); // 상호작용 시 호출되는 메서드

}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data; // 아이템 데이터 스크립터블 오브젝트

    string IInteractable.GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}";
        return str;
    }

    void IInteractable.OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data; // 플레이어의 아이템 데이터에 현재 아이템 데이터를 설정
        CharacterManager.Instance.Player.addItem?.Invoke(); // 아이템 추가 이벤트 호출
        Destroy(gameObject); // 아이템 오브젝트 제거
    }
}
