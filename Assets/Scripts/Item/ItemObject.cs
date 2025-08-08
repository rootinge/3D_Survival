using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt(); // ��ȣ�ۿ� ������Ʈ�� ��ȯ�ϴ� �޼���
    public void OnInteract(); // ��ȣ�ۿ� �� ȣ��Ǵ� �޼���

}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data; // ������ ������ ��ũ���ͺ� ������Ʈ

    string IInteractable.GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}";
        return str;
    }

    void IInteractable.OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data; // �÷��̾��� ������ �����Ϳ� ���� ������ �����͸� ����
        CharacterManager.Instance.Player.addItem?.Invoke(); // ������ �߰� �̺�Ʈ ȣ��
        Destroy(gameObject); // ������ ������Ʈ ����
    }
}
