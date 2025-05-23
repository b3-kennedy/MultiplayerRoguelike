using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CollectionBox : NetworkBehaviour
{

    public NetworkVariable<bool> isOccupied = new NetworkVariable<bool>(false);
    Dictionary<string, int> inventory = new Dictionary<string, int>();


    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += amount;
            UpdateInventoryClientRpc(itemName, inventory[itemName] + amount);
        }
        else
        {
            inventory[itemName] = amount;
            UpdateInventoryClientRpc(itemName, amount);
        }
        Debug.Log($"Deposited: {itemName}. Total: {inventory[itemName]}");
    }


    [ServerRpc(RequireOwnership = false)]
    public void InteractedWithServerRpc(bool value)
    {
        isOccupied.Value = value;
    }

    public Dictionary<string, int> GetInventory()
    {
        return inventory;
    }

    [ClientRpc]
    void UpdateInventoryClientRpc(string itemName, int amount)
    {
        if (IsServer) return;

        inventory[itemName] = amount;

        Debug.Log($"Deposited: {itemName}. Total: {inventory[itemName]}");
    }



}
