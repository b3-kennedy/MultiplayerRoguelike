using System.Collections;
using UnityEngine;

public class SwitchWeapon : MonoBehaviour
{

    Transform gunPos;
    bool isSwapping = false;
    Recoil recoil;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunPos = GetComponent<PlayerData>().GetGunParent();
        recoil = transform.Find("CameraHolder/Recoil").GetComponent<Recoil>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && gunPos.childCount > 1 && !isSwapping)
        {
            StartCoroutine(SwapGuns());

        }
    }

    IEnumerator SwapGuns()
    {
        isSwapping = true;

        Transform gun1 = gunPos.GetChild(0);
        Transform gun2 = gunPos.GetChild(1);

        Animator anim1 = gun1.GetComponent<Animator>();
        Animator anim2 = gun2.GetComponent<Animator>();

        anim1.SetTrigger("swap");

        // Wait until the swap animation is done
        float animLength = anim1.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength - 0.5f);

        gun1.GetChild(0).gameObject.SetActive(false);
        gun2.GetChild(0).gameObject.SetActive(true);

        // Perform the rest of the logic after swap is complete
        anim2.SetTrigger("unswap");
        gun2.SetAsFirstSibling();

        gunPos.GetComponent<GunSway>().UpdateGun();
        gunPos.GetComponent<GunBob>().UpdateGun();

        isSwapping = false;

        PlayerInterfaceManager playerInterfaceManager = GetComponent<PlayerInterfaceManager>();
        Gun gun = gun2.GetChild(0).GetComponent<Gun>();
        
        recoil.SetData(gun.gunData.recoilX, gun.gunData.recoilY, gun.gunData.recoilZ, gun.gunData.snap, gun.gunData.returnSpeed);
        string gunName = gun.gunData.gunName;
        playerInterfaceManager.UpdateNameText(gunName);
        playerInterfaceManager.UpdateMagText(gun.GetMagCount());
        playerInterfaceManager.UpdateAmmoText(gun.GetAmmoCount());

    }
}
