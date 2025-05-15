using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerInterfaceManager : NetworkBehaviour
{

    public GameObject ammoUI;
    public TextMeshProUGUI gunNameText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI magText;

    PlayerData playerData;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        ammoUI.SetActive(false);
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

    public void UpdateAmmoText(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    public void UpdateMagText(int mags)
    {
        magText.text = mags.ToString();
    }
}
