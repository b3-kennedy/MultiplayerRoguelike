using System.Linq;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class Interact : NetworkBehaviour
{
    [SerializeField] float interactRange = 5f;
    GameObject cam;

    PlayerInterfaceManager playerInterfaceManager;

    public LayerMask layerMask;

    Collider[] lootColliders = new Collider[20];

    public float collectRange = 5f;

    ulong netId;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
    }

    public override void OnNetworkSpawn()
    {
        cam = GetComponent<PlayerData>().GetCameraGameObject();
        playerInterfaceManager = GetComponent<PlayerInterfaceManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;


        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRange, layerMask))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                InteractWithObject(hit.collider.gameObject.layer, hit.collider.gameObject, hit.collider.tag);
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                AlternateInteract(hit.collider.tag, hit.collider.gameObject);
            }

        }

        int lootCount = Physics.OverlapSphereNonAlloc(transform.position, collectRange, lootColliders);

        for (int i = 0; i < lootCount; i++)
        {
            var col = lootColliders[i];
            string lootName = StringManager.RemoveCloneString(col.gameObject.name);

            if (ServerLootSpawner.Instance.lootDict.TryGetValue(lootName, out var lootObj))
            {
                netId = GetComponent<NetworkObject>().NetworkObjectId;
                LootCollectMove lootObject = lootColliders[i].gameObject.GetComponent<LootCollectMove>();
                lootObject.SetTargetServerRpc(netId);
                lootObject.SetCanBeCollectedServerRpc(true, NetworkManager.Singleton.LocalClientId);



            }
        }
        
        
    }

    void AlternateInteract(string tag, GameObject obj)
    {
        switch (tag)
        {
            case "Collection":
                DepositInCollectionBox(obj);
                break;
        }
    }

    void InteractWithObject(int layer, GameObject obj, string tag)
    {
        switch (layer)
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

        switch (tag)
        {
            case "Collection":
                InteractWithCollectionBox(obj);
                break;
        }


    }

    void InteractWithCollectionBox(GameObject obj)
    {


        CollectionBox box = obj.GetComponent<CollectionBox>();
        if (!box.isOccupied.Value)
        {
            playerInterfaceManager.SetUIVisible("CollectionBoxUI", true, obj);
            box.InteractedWithServerRpc(true);
        }
        else
        {
            Debug.Log("Box in use");
        }


    }

    void DepositInCollectionBox(GameObject obj)
    {
        CollectionBox box = obj.GetComponent<CollectionBox>();
        LootHolder holder = GetComponent<LootHolder>();

        foreach (var item in holder.inventory.ToList())
        {
            string itemName = item.Key;
            int amount = item.Value;

            box.AddItemServerRpc(itemName, amount);
            holder.inventory[itemName] = 0;
        }
    }

    void InteractWithGun(string gunName)
    {
        ServerInteractManager.Instance.PickUpWeaponServerRpc(gunName, NetworkManager.Singleton.LocalClientId);
    }
}
