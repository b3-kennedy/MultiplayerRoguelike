using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{

    Transform gunPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunPos = GetComponent<PlayerData>().GetGunParent();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && gunPos.childCount > 1)
        {
            gunPos.GetChild(0).GetChild(0).gameObject.SetActive(false);
            gunPos.GetChild(1).GetChild(0).gameObject.SetActive(true);
            gunPos.GetChild(1).SetAsFirstSibling();
            gunPos.GetComponent<GunSway>().UpdateGun();
            gunPos.GetComponent<GunBob>().UpdateGun();
        }
    }
}
