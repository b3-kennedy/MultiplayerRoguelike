using UnityEngine;

public class GunSway : MonoBehaviour
{
  public bool enableMouseSway = true;
    public bool enableMoveRot = true;
    public bool enableMoveOnMove = true;

   [Header("Settings")]
    public float swayAmount;
    public float maxSway;
    public float smoothing;
    public float rotSwayMultiplier;
    public float adsRotSwayMultiplier;
    public float moveMultiplier;
    float normalRotSway;

    Quaternion initialRot;
    Vector3 gunStartPos;
    Transform gun;

    Gun gunScript;



    // Start is called before the first frame update
    void Start()
    {
        initialRot = transform.localRotation;
        normalRotSway = rotSwayMultiplier;
        InitializeGun();
        

        
    }

    public void InitializeGun()
    {
        if(transform.childCount > 0)
        {
            gun = transform.GetChild(0);
            gunStartPos = gun.localPosition;
            gunScript = gun.GetChild(0).GetComponent<Gun>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gun) return;

        MouseSway();
        RotSway();
        MoveGun();
    }

    void MouseSway()
    {
        if (enableMouseSway && !gunScript.IsADS())
        {
            float mouseX = Input.GetAxis("Mouse X") * swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

            mouseX = Mathf.Clamp(mouseX, -maxSway, maxSway);
            mouseY = Mathf.Clamp(mouseY, -maxSway, maxSway);

            Quaternion targetRotX = Quaternion.AngleAxis(-mouseX, Vector3.up);
            Quaternion targetRotY = Quaternion.AngleAxis(mouseY, Vector3.right);
            Quaternion targetRot = initialRot * targetRotX * targetRotY;

            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, Time.deltaTime * smoothing);
        }

    }

    void RotSway()
    {
        if (enableMoveRot)
        {
            if (gunScript.IsADS())
            {
                rotSwayMultiplier = adsRotSwayMultiplier;
            }
            else
            {
                rotSwayMultiplier = normalRotSway;
            }
            float moveX = Input.GetAxis("Horizontal");
            Quaternion newRot = new Quaternion(gun.localRotation.x, gun.localRotation.y, moveX * rotSwayMultiplier, gun.localRotation.w);
            gun.localRotation = Quaternion.Lerp(gun.localRotation, newRot, Time.deltaTime * 10);
        }

    }

    void MoveGun()
    {
        if (enableMoveOnMove)
        {
            float moveZ = Input.GetAxisRaw("Vertical");

            if (moveZ != 0)
            {
                float zPos = gun.localPosition.z + (moveZ * moveMultiplier);
                zPos = Mathf.Clamp(zPos, -0.37f, -0.33f);
                Vector3 newPos = new Vector3(gun.localPosition.x, gun.localPosition.y, zPos);
                gun.localPosition = Vector3.Lerp(gun.localPosition, newPos, Time.deltaTime * 10);
            }
            else
            {
                gun.localPosition = Vector3.Lerp(gun.localPosition, gunStartPos, Time.deltaTime * 10);
            }
        }



    }
}
