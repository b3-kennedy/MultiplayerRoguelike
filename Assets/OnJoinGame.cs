using Unity.Netcode;
using UnityEngine;

public class OnJoinGame : NetworkBehaviour
{
    public GameObject defaultGun;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SpawnDefaultGunServerRpc(OwnerClientId);
        }
    }

    [ServerRpc]
    private void SpawnDefaultGunServerRpc(ulong clientId)
    {
        GameObject gun = Instantiate(defaultGun);
        NetworkObject gunNetObj = gun.GetComponent<NetworkObject>();

        gunNetObj.SpawnWithOwnership(clientId);

        // Delay the client-side parenting until everything is fully spawned
        AttachGunClientRpc(gunNetObj.NetworkObjectId, clientId);
    }

    [ClientRpc]
    private void AttachGunClientRpc(ulong gunNetId, ulong targetClientId)
    {
        // Only run on the target client (owner of the gun)
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;

        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(gunNetId, out NetworkObject gunObj))
        {
            Transform gunHold = transform.GetChild(1).GetChild(0); // Adjust this path as needed
            gunObj.transform.SetParent(gunHold, false); // Local, visual-only parenting
            gunObj.transform.localPosition = Vector3.zero;
            gunObj.transform.localRotation = Quaternion.identity;
        }
    }
}
