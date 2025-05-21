using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Loot
{
    public GameObject lootObject;
    public float chance;
}

[CreateAssetMenu(fileName = "LootTable", menuName = "Scriptable Objects/LootTable")]
public class LootTable : ScriptableObject
{
    public List<Loot> lootTable;
}
