using Unity.Netcode;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GunData gunData;
    bool isAiming;
    float aimProgress;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localPosition = gunData.position;
    }

    // Update is called once per frame
    void Update()
    {
        Aiming();
    }

    public bool IsADS()
    {
        return isAiming;
    }

    void Aiming()
    {
        if(Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
        }
        else if(Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
        }
        
        if (isAiming)
        {
            //PlayerUI.Instance.crosshair.SetActive(false);
            aimProgress += Time.deltaTime * gunData.adsTime;

        }
        else
        {
            //PlayerUI.Instance.crosshair.SetActive(true);
            aimProgress -= Time.deltaTime * gunData.adsTime;
        }

        aimProgress = Mathf.Clamp01(aimProgress);

        transform.localPosition = Vector3.Lerp(gunData.position, gunData.adsPosition, aimProgress);
    }
}
