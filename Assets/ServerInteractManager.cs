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
        if (guns.TryGetValue(gunName, out var gun))
        {
            Debug.Log(gunName);
            NetworkObject clientPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            Transform gunPos = clientPlayer.transform.Find("CameraHolder/Recoil/Camera/GunPosition");
            Recoil recoil = clientPlayer.transform.Find("CameraHolder/Recoil").GetComponent<Recoil>();
            GameObject spawnedGun = Instantiate(gun, gunPos);

            if (gunPos.childCount > 1)
            {
                spawnedGun.transform.SetAsFirstSibling();
                gunPos.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            }

            Gun gunScript = spawnedGun.transform.GetChild(0).GetComponent<Gun>();
            recoil.SetData(gunScript.gunData.recoilX, gunScript.gunData.recoilY, gunScript.gunData.recoilZ, gunScript.gunData.snap, gunScript.gunData.returnSpeed);
            gunPos.GetComponent<GunSway>().InitializeGun();
            gunPos.GetComponent<GunBob>().InitializeGun();
            spawnedGun.GetComponent<Animator>().SetTrigger("pickedup");
            clientPlayer.GetComponent<PlayerInterfaceManager>().OnGunPickup(spawnedGun);
            
        }
    }
}
