using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : NetworkBehaviour
{
    NavMeshAgent agent;
    Transform target;

    NavMeshPath path;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            NetworkObject clientPlayer = NetworkManager.Singleton.ConnectedClients[0].PlayerObject;
            target = clientPlayer.transform;
            Debug.Log(target);
        }
        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        agent.CalculatePath(target.position, path);
        agent.SetPath(path);
    }
}
