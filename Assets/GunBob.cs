using UnityEngine;

[RequireComponent(typeof(PositionFollower))]
public class GunBob : MonoBehaviour
{

    public float intensity;
    public float intensityX;

    public float adsIntensity = 0.0005f;
    public float adsIntensityX = 0.2f;
    public float effectSpeed;
    
    float normalIntensity;
    float normalIntensityX;

    PositionFollower follower;
    Vector3 originalOffset;
    float sinTime;

    GunData gunData;

    Gun gun;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        follower = GetComponent<PositionFollower>();
        originalOffset = follower.offset;
        normalIntensity = intensity;
        normalIntensityX = intensityX;
        gunData = transform.GetChild(0).GetComponent<Gun>().gunData;
        gun = transform.GetChild(0).GetComponent<Gun>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (gun.IsADS())
        {
            intensityX = adsIntensityX;
            intensity = adsIntensity;
            
        }
        else
        {
            intensity = normalIntensity;
            intensityX = normalIntensityX;
        }

        Vector3 input = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
        if(input.magnitude > 0f)
        {
            sinTime += Time.deltaTime * effectSpeed;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 10);
        }

        float sinY = -Mathf.Abs(intensity * Mathf.Sin(sinTime));
        Vector3 sinX = intensity * intensityX * Mathf.Cos(sinTime) * follower.transform.right;

        follower.offset = new Vector3
        {
            x = originalOffset.x,
            y = originalOffset.y + sinY,
            z = originalOffset.z
        };

        follower.offset += sinX;
    }
}
