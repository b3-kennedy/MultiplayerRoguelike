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
    Button convertButton;
    Button craftButton;

    GameObject convertPanel;
    GameObject craftPanel;

    GameObject collectionBox;

    TextMeshProUGUI greenResourceText;
    TextMeshProUGUI redResourceText;
    TextMeshProUGUI blueResourceText;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        ammoUI.SetActive(false);
        collectionUI.SetActive(false);

        convertButton = collectionUI.transform.Find("ConvertButton").GetComponent<Button>();
        craftButton = collectionUI.transform.Find("CraftButton").GetComponent<Button>();

        convertPanel = collectionUI.transform.Find("ConvertPanel").gameObject;
        craftPanel = collectionUI.transform.Find("CraftPanel").gameObject;

        convertButton.onClick.AddListener(delegate { SetUIVisible("ConvertPanel"); });
        craftButton.onClick.AddListener(delegate { SetUIVisible("CraftPanel"); });

        greenResourceText = collectionUI.transform.Find("ConvertPanel/ResourceTextParent/Green").GetComponent<TextMeshProUGUI>();
        redResourceText = collectionUI.transform.Find("ConvertPanel/ResourceTextParent/Red").GetComponent<TextMeshProUGUI>();
        blueResourceText = collectionUI.transform.Find("ConvertPanel/ResourceTextParent/Blue").GetComponent<TextMeshProUGUI>();
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
        }
    }

    public void SetUIVisible(string name, bool value = true, GameObject interactedWithObject = null)
    {
        GetComponent<PlayerLook>().enabled = false;
        switch (name)
        {
            case "CollectionBoxUI":

                if (interactedWithObject)
                {
                    collectionBox = interactedWithObject;
                    CollectionBox box = collectionBox.GetComponent<CollectionBox>();
                    Dictionary<string, int> inventory = box.GetInventory();

                    int greenCount = 0, blueCount = 0, redCount = 0;

                    if (inventory.TryGetValue("GreenOrganicMatter", out greenCount))
                    {
                        greenResourceText.text = "Green: " + greenCount;
                    }
                    else
                    {
                        greenResourceText.text = "Green: 0";
                    }

                    if (inventory.TryGetValue("BlueOrganicMatter", out blueCount))
                    {
                        blueResourceText.text = "Blue: " + blueCount;
                    }
                    else
                    {
                        blueResourceText.text = "Blue: 0";
                    }

                    if (inventory.TryGetValue("RedOrganicMatter", out redCount))
                    {
                        redResourceText.text = "Red: " + redCount;
                    }
                    else
                    {
                        redResourceText.text = "Red: 0";
                    }
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
                break;
            case "ConvertPanel":
                craftPanel.SetActive(false);
                convertPanel.SetActive(true);
                break;
            case "CraftPanel":
                convertPanel.SetActive(false);
                craftPanel.SetActive(true);
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
