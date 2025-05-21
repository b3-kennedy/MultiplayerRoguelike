using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class Gun : MonoBehaviour
{

    public GunData gunData;
    protected bool isAiming;
    protected float aimProgress;

    protected float shootTimer;

    public LayerMask layerMask;

    PlayerData playerData;

    GameObject cam;

    Recoil recoil;

    protected int ammo;

    protected int magCount;

    protected Animator anim;

    bool isReloading = false;

    protected PlayerInterfaceManager playerInterfaceManager;

    float reloadTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.parent.parent && transform.parent.parent.name == "GunPosition")
        {
            ammo = gunData.magazineSize;
            cam = transform.parent.parent.parent.gameObject;
            magCount = 3;
            playerData = transform.parent.parent.parent.parent.parent.parent.GetComponent<PlayerData>();
            recoil = transform.parent.parent.parent.parent.GetComponent<Recoil>();
            recoil.SetData(gunData.recoilX, gunData.recoilY, gunData.recoilZ, gunData.snap, gunData.returnSpeed);
            transform.parent.GetComponent<Rigidbody>().isKinematic = true;
            transform.parent.GetComponent<Collider>().enabled = false;
            transform.parent.localPosition = gunData.position;
            shootTimer = gunData.fireRate;
            anim = transform.parent.GetComponent<Animator>();
            if (playerInterfaceManager)
            {
                playerInterfaceManager.UpdateAmmoText(ammo);
                playerInterfaceManager.UpdateMagText(magCount);
            }

        }
        else if (!transform.parent)
        {
            enabled = false;
            transform.parent.GetComponent<Rigidbody>().isKinematic = false;
            transform.parent.GetComponent<Collider>().enabled = true;
        }
        
    }

    public void SetPlayerInterface(PlayerInterfaceManager manager)
    {
        playerInterfaceManager = manager;
    }

    public PlayerInterfaceManager GetPlayerInterfaceManager()
    {
        return playerInterfaceManager;
    }

    public int GetAmmoCount()
    {
        return ammo;
    }

    public int GetMagCount()
    {
        return magCount;
    }


    // Update is called once per frame
    void Update()
    {
        if (!playerData || !playerData.GetOwnership()) return;

        Aiming();
        Shoot();
        Reload();        
    }

    public void Recoil()
    {
        recoil.RecoilFire();
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && magCount > 0)
        {
            anim.SetTrigger("reload");
            isReloading = true;
        }

        if (isReloading)
        {
            reloadTimer += Time.deltaTime;
            if (reloadTimer >= gunData.reloadTime)
            {
                anim.SetTrigger("unreload");
                isReloading = false;
                ammo = gunData.magazineSize;
                reloadTimer = 0;
                magCount--;
                playerInterfaceManager.UpdateMagText(magCount);
                playerInterfaceManager.UpdateAmmoText(ammo);

            }
         }
    }

    public virtual void Shoot()
    {

    }

    public bool IsADS()
    {
        return isAiming;
    }

    public virtual void Raycast()
    {
        RaycastHit[] colliders = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 1000, layerMask);

        int enemiesHit = 0;
        foreach (var hit in colliders)
        {
            if (hit.collider.GetComponent<Health>())
            {
                enemiesHit++;
                ulong id = hit.collider.GetComponent<NetworkObject>().NetworkObjectId;
                ServerManager.Instance.DealDamageServerRpc(id, gunData.damage/enemiesHit);
                
            }
            else if (hit.collider.CompareTag("Head"))
            {
                Health health = hit.collider.transform.parent.GetComponent<Health>();
                if (health)
                {
                    enemiesHit++;
                    ulong id = hit.collider.transform.parent.GetComponent<NetworkObject>().NetworkObjectId;
                    ServerManager.Instance.DealDamageServerRpc(id, gunData.damage/enemiesHit * 1.5f);
                    
                }
            }

            if (enemiesHit >= gunData.penetrationLimit)
            {
                break;    
            }
        }
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
            aimProgress += Time.deltaTime * gunData.adsTime;

        }
        else
        {
            aimProgress -= Time.deltaTime * gunData.adsTime;
        }

        aimProgress = Mathf.Clamp01(aimProgress);

        transform.parent.localPosition = Vector3.Lerp(gunData.position, gunData.adsPosition, aimProgress);
    }
}
