using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public bool GetOwnership()
    {
        return IsOwner;
    }

    public GameObject GetCameraGameObject()
    {
        return transform.GetChild(1).GetChild(0).GetChild(0).gameObject;
    }
}
