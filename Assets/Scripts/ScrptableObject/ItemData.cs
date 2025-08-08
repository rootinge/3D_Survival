using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  ItemType
{
    Equipable,
    Consumable,
    Resource
}

public enum ConsumableType
{
    Health,
    Hunger
}

[Serializable]
public class ItemDataConsumbale
{
    public ConsumableType type; // �Һ� ������ ���� (Health, Hunger ��)
    public float value; // �Һ��� �� ȸ���Ǵ� ��
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName; // ������ �̸�
    public string description; // ������ ����
    public ItemType Type; // ������ ����
    public Sprite icon; // ������ ������
    public GameObject dropPrefab; // ������ ��� ������

    [Header("Stacking")]
    public bool canStack; // �������� ���� �� �ִ��� ����
    public int maxStackAmount; // �ִ� ���� �� �ִ� ��

    [Header("Consumable")]
    public ItemDataConsumbale[] consumbales; // �Һ� ������ ���� (Health, Hunger ��) 

    [Header("Equip")]
    public GameObject equipPrefab; // ���� ������
}
