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
    public ConsumableType type; // 소비 아이템 종류 (Health, Hunger 등)
    public float value; // 소비할 때 회복되는 값
}


[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName; // 아이템 이름
    public string description; // 아이템 설명
    public ItemType Type; // 아이템 종류
    public Sprite icon; // 아이템 아이콘
    public GameObject dropPrefab; // 아이템 드랍 프리팹

    [Header("Stacking")]
    public bool canStack; // 아이템이 쌓일 수 있는지 여부
    public int maxStackAmount; // 최대 쌓을 수 있는 양

    [Header("Consumable")]
    public ItemDataConsumbale[] consumbales; // 소비 아이템 정보 (Health, Hunger 등) 

    [Header("Equip")]
    public GameObject equipPrefab; // 장착 프리팹
}
