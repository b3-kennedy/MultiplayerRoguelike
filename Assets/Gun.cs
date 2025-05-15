using Unity.Netcode;
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

    protected Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (transform.parent && transform.parent.parent.name == "GunPosition")
        {
            ammo = gunData.magazineSize;
            cam = transform.parent.parent.parent.gameObject;
            playerData = transform.parent.parent.parent.parent.parent.parent.GetComponent<PlayerData>();
            recoil = transform.parent.parent.parent.parent.GetComponent<Recoil>();
            recoil.SetData(gunData.recoilX, gunData.recoilY, gunData.recoilZ, gunData.snap, gunData.returnSpeed);
            transform.parent.GetComponent<Rigidbody>().isKinematic = true;
            transform.parent.GetComponent<Collider>().enabled = false;
            transform.parent.localPosition = gunData.position;
            shootTimer = gunData.fireRate;
            anim = transform.parent.GetComponent<Animator>();
        }
        else if (!transform.parent)
        {
            enabled = false;
            transform.parent.GetComponent<Rigidbody>().isKinematic = false;
            transform.parent.GetComponent<Collider>().enabled = true;
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        if(!playerData || !playerData.GetOwnership()) return;

        Aiming();
        Shoot();
    }

    public void Recoil()
    {
        recoil.RecoilFire();
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
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, 1000,layerMask))
        {
            if(hit.collider.GetComponent<Health>())
            {
                ulong id = hit.collider.GetComponent<NetworkObject>().NetworkObjectId;
                ServerManager.Instance.DealDamageServerRpc(id, gunData.damage);
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
