using Unity.Netcode;
using UnityEngine;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if(transform.parent && transform.parent.name == "GunPosition")
        {
            cam = transform.parent.parent.gameObject;
            playerData = transform.parent.parent.parent.GetComponent<PlayerData>();
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
            transform.localPosition = gunData.position;
            shootTimer = gunData.fireRate;
        }
        else if(!transform.parent)
        {
            enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Collider>().enabled = true;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!playerData || !playerData.GetOwnership()) return;

        Aiming();
        Shoot();
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
