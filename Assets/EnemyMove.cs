using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : NetworkBehaviour
{
    NavMeshAgent agent;
    Transform target;
    NavMeshPath path;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return; // Ensure only server controls enemy movement

        if (NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            NetworkObject clientPlayer = NetworkManager.Singleton.ConnectedClients[0].PlayerObject;
            target = clientPlayer.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        path = new NavMeshPath();
    }

    void Update()
    {
        if (!IsServer || target == null) return;

        // Handle off-mesh link traversal manually
        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(HandleOffMeshLink());
            return; // Wait for off-mesh link completion before setting path again
        }

        agent.CalculatePath(target.position, path);
        agent.SetPath(path);
    }

    IEnumerator HandleOffMeshLink()
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        float duration = 1f; // You can adjust this
        float time = 0f;

        while (time < duration)
        {
            agent.transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        agent.transform.position = endPos;
        agent.CompleteOffMeshLink(); // Tell the agent the link has been traversed
    }
}