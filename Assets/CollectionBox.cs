using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CollectionBox : NetworkBehaviour
{
    Dictionary<string, int> inventory = new Dictionary<string, int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
        }
        else
        {
            inventory[itemName] = amount;
        }
        Debug.Log($"Picked up: {itemName}. Total: {inventory[itemName]}");
    }


}
