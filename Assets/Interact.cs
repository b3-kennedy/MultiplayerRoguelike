using Unity.Netcode;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class Interact : MonoBehaviour
{
    [SerializeField] float interactRange = 5f;
    GameObject cam;

    PlayerInterfaceManager playerInterfaceManager;

    public LayerMask layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<PlayerData>().GetCameraGameObject();
        playerInterfaceManager = GetComponent<PlayerInterfaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit,interactRange, layerMask))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                InteractWithObject(hit.collider.gameObject.layer, hit.collider.gameObject);
            }

        }
    }

    void InteractWithObject(int layer, GameObject obj)
    {
            switch(layer)
            {
                case 6:
                    Debug.Log("Interacted with gun");
                    InteractWithGun(obj.name);
                    Destroy(obj);
                    break;
                default:
                    Debug.Log(obj.name);
                    break;

            }
    }

    void InteractWithGun(string gunName)
    {
        ServerInteractManager.Instance.PickUpWeaponServerRpc(gunName, NetworkManager.Singleton.LocalClientId);
    }
}
