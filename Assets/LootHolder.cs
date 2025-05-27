using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LootHolder : NetworkBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    void Start()
    {
        ServerInteractManager.Instance.PickUpWeaponServerRpc("Glock", NetworkManager.Singleton.LocalClientId);
    }

    public override void OnNetworkSpawn()
    {
        AddItemServerRpc("Ammo", NetworkManager.Singleton.LocalClientId, 100);
        
        Transform gunParent = GetComponent<PlayerData>().GetGunParent();
        for (int i = 0; i < gunParent.childCount; i++)
        {
            Gun gun = gunParent.GetChild(i).GetChild(0).GetComponent<Gun>();
            gun.UpdateAmmoCount();
            GetComponent<PlayerInterfaceManager>().UpdateMagText(gun.GetMagCount());
            GetComponent<PlayerInterfaceManager>().UpdateAmmoText(gun.GetAmmoCount());
        }
    }

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

        if (itemName == "Ammo")
        {
            Transform gunParent = GetComponent<PlayerData>().GetGunParent();
            for (int i = 0; i < gunParent.childCount; i++)
            {
                Gun gun = gunParent.GetChild(i).GetChild(0).GetComponent<Gun>();
                gun.UpdateAmmoCount();
                GetComponent<PlayerInterfaceManager>().UpdateMagText(gun.GetMagCount());
                GetComponent<PlayerInterfaceManager>().UpdateAmmoText(gun.GetAmmoCount());
            }

        }

        Debug.Log($"Picked up: {itemName}. Total: {inventory[itemName]}");
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

}
