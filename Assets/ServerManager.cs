using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    public static ServerManager Instance { get; private set; }

    void Awake()
    {
        Debug.Log("hello");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        
    }


    [ServerRpc(RequireOwnership = false)]
    public void DealDamageServerRpc(ulong objectId, float damage)
    {
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var obj))
        {
            Health healthScript = obj.GetComponent<Health>();
            if(healthScript)
            {
                healthScript.TakeDamageClientRpc(damage);
            }
        }
    }
}


