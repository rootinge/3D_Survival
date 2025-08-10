using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive; // 아이템 데이터
    public int quantityPerHit = 1; // 한 번의 상호작용으로 주는 아이템 수
    public int capacy; // 몇번 때릴 수 있는지

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
