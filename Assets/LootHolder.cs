using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LootHolder : NetworkBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(string itemName, ulong clientId, int count)
    {
        AddItemClientRpc(itemName, clientId, count);
    }

    [ClientRpc]
    void AddItemClientRpc(string itemName, ulong clientId, int count)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += count;
        }
        else
        {
            inventory[itemName] = count;
        }

        Debug.Log($"Picked up: {itemName}. Total: {inventory[itemName]}");
    }

}
