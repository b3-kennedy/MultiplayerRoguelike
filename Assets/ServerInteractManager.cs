using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerInteractManager : NetworkBehaviour
{
    public static ServerInteractManager Instance {get; private set;}

    Dictionary<string, GameObject> guns = new Dictionary<string, GameObject>();

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        GameObject[] loadedGuns = Resources.LoadAll<GameObject>("Guns");

        foreach (GameObject gun in loadedGuns)
        {
            if (!guns.ContainsKey(gun.name))
            {
                guns.Add(gun.name, gun);
            }
            else
            {
                Debug.LogWarning($"Duplicate gun name found: {gun.name}. Skipping.");
            }
        }

        Debug.Log($"Loaded {guns.Count} guns into the dictionary.");        
    }

    [ServerRpc(RequireOwnership = false)]
    public void PickUpWeaponServerRpc(string gunName, ulong clientId)
    {
        
        PickUpWeaponClientRpc(gunName, clientId);
    }

    [ClientRpc]
    public void PickUpWeaponClientRpc(string gunName, ulong clientId)
    {
        if(guns.TryGetValue(gunName, out var gun))
        {
            Debug.Log(gunName);
            Transform gunPos = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform.Find("CameraHolder/Recoil/Camera/GunPosition");
            GameObject spawnedGun = Instantiate(gun, gunPos);
            gunPos.GetComponent<GunSway>().InitializeGun();
            gunPos.GetComponent<GunBob>().InitializeGun();
        }
    }
}
