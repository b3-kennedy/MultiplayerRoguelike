using JetBrains.Annotations;
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

    Collider[] lootColliders = new Collider[20];

    public float collectRange = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<PlayerData>().GetCameraGameObject();
        playerInterfaceManager = GetComponent<PlayerInterfaceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRange, layerMask))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                InteractWithObject(hit.collider.gameObject.layer, hit.collider.gameObject);
            }

        }

        int lootCount = Physics.OverlapSphereNonAlloc(transform.position, collectRange, lootColliders);

        for (int i = 0; i < lootCount; i++)
        {
            var col = lootColliders[i];
            string lootName = col.gameObject.name.Replace("(Clone)", "").Trim();

            if (ServerLootSpawner.Instance.lootDict.TryGetValue(lootName, out var lootObj))
            {
                LootCollectMove lootObject = lootColliders[i].gameObject.GetComponent<LootCollectMove>();
                lootObject.SetTarget(transform);
                lootObject.SetCanBeCollected(true);
                lootObject.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                lootColliders[i].isTrigger = true;


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
