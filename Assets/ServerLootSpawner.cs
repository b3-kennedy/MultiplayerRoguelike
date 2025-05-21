using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerLootSpawner : NetworkBehaviour
{

    public static ServerLootSpawner Instance;
    public Dictionary<string, GameObject> lootDict = new Dictionary<string, GameObject>();


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] loot = Resources.LoadAll<GameObject>("Loot/LootObjects");
        Debug.Log($"Loaded {loot.Length} loot prefabs.");

        foreach (GameObject obj in loot)
        {
            if (!lootDict.ContainsKey(obj.name))
            {
                lootDict.Add(obj.name, obj);
            }
            else
            {
                Debug.LogWarning($"Duplicate loot prefab name: {obj.name}, skipping.");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnLootServerRpc(string name, float x, float y, float z)
    {
        if (lootDict.TryGetValue(name, out var lootObject))
        {
            Vector3 pos = new Vector3(x, y, z);
            GameObject spawnedLoot = Instantiate(lootObject, pos, Quaternion.identity);
            spawnedLoot.GetComponent<NetworkObject>().Spawn();
        }
    }


}
