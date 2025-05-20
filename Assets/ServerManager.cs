using System.Collections.Generic;
using System.Data.Common;
using Unity.AI.Navigation;
using Unity.Netcode;
using UnityEngine;

public class ServerManager : NetworkBehaviour
{
    public static ServerManager Instance { get; private set; }

    float enemySpawnTimer;

    public float enemySpawnInterval;

    public GameObject enemyPrefab;

    public NavMeshSurface surface;

    public bool spawnEnemies;

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

    }

    void Update()
    {
        if (!IsServer) return;

        if (spawnEnemies)
        {
            SpawnEnemies();
         }


    }

    void SpawnEnemies()
    {
        enemySpawnTimer += Time.deltaTime;
        if (enemySpawnTimer >= enemySpawnInterval)
        {
            SpawnEnemyServerRpc();
            enemySpawnTimer = 0;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void DealDamageServerRpc(ulong objectId, float damage)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var obj))
        {
            Health healthScript = obj.GetComponent<Health>();
            if (healthScript)
            {
                healthScript.TakeDamageClientRpc(damage);
            }
        }
    }

    [ServerRpc]
    public void SpawnEnemyServerRpc()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        enemy.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    public void BuildNavmeshServerRpc()
    {
        surface.BuildNavMesh();
    }
    
}


