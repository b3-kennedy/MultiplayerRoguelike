using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceManager : NetworkBehaviour
{

    public GameObject ammoUI;
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;

    

    PlayerData playerData;

    GameObject currentActiveUI;

    [Header("Collection Box Interface")]
    public GameObject collectionUI;
    [SerializeField] Button convertButton;
    [SerializeField] Button craftButton;

    [SerializeField] GameObject convertPanel;
    [SerializeField] GameObject craftPanel;

    [SerializeField] GameObject buildingMaterialsPanel;

    [SerializeField] GameObject weaponMaterialsPanel;

    [SerializeField] Button buildingMaterialsButton;
    [SerializeField] Button weaponMaterialsButton;

    GameObject collectionBox;

    [SerializeField] TextMeshProUGUI greenResourceText;
    [SerializeField] TextMeshProUGUI redResourceText;
    [SerializeField] TextMeshProUGUI blueResourceText;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        ammoUI.SetActive(false);
        collectionUI.SetActive(false);

        convertButton.onClick.AddListener(delegate { SetUIVisible("ConvertPanel"); });
        craftButton.onClick.AddListener(delegate { SetUIVisible("CraftPanel"); });

        weaponMaterialsPanel.SetActive(false);
        buildingMaterialsPanel.SetActive(true);

        weaponMaterialsButton.onClick.AddListener(delegate { SetUIVisible("WeaponMaterialsPanel"); });
        buildingMaterialsButton.onClick.AddListener(delegate { SetUIVisible("BuildingMaterialsPanel"); });
    }

    public void OnGunPickup(GameObject gun)
    {
        if (!IsOwner) return;

        Gun g = gun.transform.GetChild(0).GetComponent<Gun>();
        GunData gData = g.gunData;

        g.SetPlayerInterface(this);

        gunNameText.text = gData.gunName;

        ammoUI.SetActive(true);

    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Escape) && currentActiveUI)
        {
            SetUIVisible(currentActiveUI.name, false);
            Gun gun = GetComponent<PlayerData>().GetGunParent().GetChild(0).GetChild(0).GetComponent<Gun>();
            gun.SetCanShoot(true);
        }
    }

    public void AddCraftedItemToCollectionBox(List<RecipeItemAndCount> items, int quantity, string itemName)
    {
        if (!collectionBox) return;

        Dictionary<string, int> inventory = collectionBox.GetComponent<CollectionBox>().GetInventory();
        foreach (var item in items)
        {
            if (inventory.ContainsKey(item.item.name) && inventory[item.item.name] >= item.count)
            {
                collectionBox.GetComponent<CollectionBox>().AddItemServerRpc(itemName, quantity);
                collectionBox.GetComponent<CollectionBox>().RemoveItemServerRpc(item.item.name, item.count);
            }
        }
        UpdateItemCount();
        
        
    }

    public void UpdateItemCount()
    {
        CollectionBox box = collectionBox.GetComponent<CollectionBox>();
        Dictionary<string, int> inventory = box.GetInventory();



        if (inventory.TryGetValue("Green", out var greenCount))
        {
            greenResourceText.text = "Green: " + greenCount;
        }
        else
        {
            greenResourceText.text = "Green: 0";
        }

        if (inventory.TryGetValue("Blue", out var blueCount))
        {
            blueResourceText.text = "Blue: " + blueCount;
        }
        else
        {
            blueResourceText.text = "Blue: 0";
        }

        if (inventory.TryGetValue("Red", out var redCount))
        {
            redResourceText.text = "Red: " + redCount;
        }
        else
        {
            redResourceText.text = "Red: 0";
        }

    }

    public void SetUIVisible(string name, bool value = true, GameObject interactedWithObject = null)
    {
        Gun gun = GetComponent<PlayerData>().GetGunParent().GetChild(0).GetChild(0).GetComponent<Gun>();
        gun.SetCanShoot(false);
        Debug.Log(gun);
        GetComponent<PlayerLook>().enabled = false;
        switch (name)
        {
            case "CollectionBoxUI":
                if (interactedWithObject)
                {
                    collectionBox = interactedWithObject;
                    UpdateItemCount();
                }
                if (value)
                {
                    Cursor.lockState = CursorLockMode.None;
                    currentActiveUI = collectionUI;
                    collectionUI.SetActive(true);
                    convertPanel.SetActive(true);
                    craftPanel.SetActive(false);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    collectionUI.SetActive(false);
                    GetComponent<PlayerLook>().enabled = true;
                    if (collectionBox)
                    {
                        collectionBox.GetComponent<CollectionBox>().InteractedWithServerRpc(false);
                    }

                }
                GetComponent<Crafting>().ShowRecipes();
                break;
            case "ConvertPanel":
                craftPanel.SetActive(false);
                convertPanel.SetActive(true);
                break;
            case "CraftPanel":
                convertPanel.SetActive(false);
                craftPanel.SetActive(true);
                break;
            case "BuildingMaterialsPanel":
                weaponMaterialsPanel.SetActive(false);
                buildingMaterialsPanel.SetActive(true);
                break;
            case "WeaponMaterialsPanel":
                buildingMaterialsPanel.SetActive(false);
                weaponMaterialsPanel.SetActive(true);
                break;
            default:
                Debug.Log($"UI element {name} not found, perhaps a typo in the case statement?");
                GetComponent<PlayerLook>().enabled = true;
                break;
        }
    }

    


    public void UpdateNameText(string name)
    {
        gunNameText.text = name;
    }

    public void UpdateAmmoText(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    public void UpdateMagText(int mags)
    {
        magText.text = mags.ToString();
    }
}
