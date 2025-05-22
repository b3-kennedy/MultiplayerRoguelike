using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LootHolder : NetworkBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    [ServerRpc(RequireOwnership = false)]
    public void AddItemServerRpc(string itemName, ulong clientId)
    {
        AddItemClientRpc(itemName, clientId);
    }

    [ClientRpc]
    void AddItemClientRpc(string itemName, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName]++;
        }
        else
        {
            inventory[itemName] = 1;
        }

        Debug.Log($"Picked up: {itemName}. Total: {inventory[itemName]}");
    }
}
