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
            UpdateInventoryClientRpc(itemName, inventory[itemName] + amount);
        }
        else
        {
            UpdateInventoryClientRpc(itemName, amount);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveItemServerRpc(string itemName, int amount)
    {
        if (inventory.ContainsKey(itemName))
        {
            UpdateInventoryClientRpc(itemName, inventory[itemName] - amount);
        }
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
        //if (IsServer) return;

 
        inventory[itemName] = amount;

        if (inventory[itemName] < 0)
        {
            inventory[itemName] = 0;
        }

        Debug.Log($"Deposited: {itemName}. Total: {inventory[itemName]}");
    }



}
