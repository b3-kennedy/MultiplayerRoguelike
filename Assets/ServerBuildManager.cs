using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerBuildManager : NetworkBehaviour
{

    public static ServerBuildManager Instance;

    public Dictionary<string, GameObject> blocks = new Dictionary<string, GameObject>();

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
        GameObject[] loadedBlocks = Resources.LoadAll<GameObject>("BuildingBlocks");

        foreach (GameObject block in loadedBlocks)
        {
            if (!blocks.ContainsKey(block.name))
            {
                blocks.Add(block.name, block);
            }
            else
            {
                Debug.LogWarning($"Duplicate gun name found: {block.name}. Skipping.");
            }
        }

        Debug.Log($"Loaded {blocks.Count} blocks into the dictionary."); 
    }

    [ServerRpc(RequireOwnership = false)]
    public void BuildServerRpc(string blockName, float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        Debug.Log(blockName);
        if(blocks.TryGetValue(blockName, out var block))
        {
            Vector3 pos = new Vector3(posX,posY,posZ);
            Quaternion rot = new Quaternion(rotX, rotY, rotZ, rotW);
            GameObject spawnedBlock = Instantiate(block, pos, rot);
            spawnedBlock.GetComponent<NetworkObject>().Spawn();
        }
    }

    
}
