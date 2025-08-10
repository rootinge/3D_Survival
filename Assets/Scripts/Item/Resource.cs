using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive; // ������ ������
    public int quantityPerHit = 1; // �� ���� ��ȣ�ۿ����� �ִ� ������ ��
    public int capacy; // ��� ���� �� �ִ���

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for(int i = 0; i < quantityPerHit; i++)
        {
            if (capacy < 0) return;
            capacy--;
            Instantiate(itemToGive.dropPrefab,hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
        }
    }
}
