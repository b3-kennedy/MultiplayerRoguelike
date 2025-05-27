using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CollectionBox : NetworkBehaviour
{

    public static CollectionBox Instance;
    public NetworkVariable<bool> isOccupied = new NetworkVariable<bool>(false);
    Dictionary<string, int> inventory = new Dictionary<string, int>();


    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

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
        Debug.Log(amount);
        if (inventory.ContainsKey(itemName))
        {
            UpdateInventoryClientRpc(itemName, inventory[itemName] - amount);
        }
    }

    public void DebugInventory()
    {
        if (inventory.Count == 0)
        {
            Debug.Log("Inventory is empty.");
        }
        else
        {
            Debug.Log("Inventory contents:");
            foreach (KeyValuePair<string, int> entry in inventory)
            {
                Debug.Log($" - {entry.Key}: {entry.Value}");
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void InteractedWithServerRpc(bool value)
    {
        //isOccupied.Value = value;
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
