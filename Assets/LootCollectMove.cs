using Unity.Netcode;
using UnityEngine;

public class LootCollectMove : NetworkBehaviour
{
    public bool canBeCollected = false;
    public Transform target;

    public float lerpSpeed = 5f;

    ulong clientId;

    [ServerRpc(RequireOwnership = false)]
    public void SetCanBeCollectedServerRpc(bool value, ulong cId)
    {
        canBeCollected = value;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        clientId = cId;
    }

    public bool GetCanBeCollectedValue()
    {
        return canBeCollected;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetTargetServerRpc(ulong networkId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkId, out var t))
        {
            target = t.transform;
        }
        
    }

    public Transform GetTarget()
    {
        return target;
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsServer) return;

        if (canBeCollected && target)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed * Time.deltaTime);
        }

        if (target && Vector3.Distance(transform.position, target.position) <= 1f)
        {
            LootHolder holder = target.gameObject.GetComponent<LootHolder>();
            if (holder)
            {
                holder.AddItemServerRpc(StringManager.RemoveCloneString(gameObject.name), clientId);
            }
            DestroyDropServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyDropServerRpc()
    {
        Destroy(gameObject);
    }
}
