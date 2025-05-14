using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Recoil : MonoBehaviour
{
     Vector3 currentRot;
     Vector3 targetRot;

     float recoilX;
     float recoilY;
     float recoilZ;

     float snap;
     float returnSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetData(float recX, float recY, float recZ, float snappiness, float returnSpd)
    {
        recoilX = recX;
        recoilY = recY;
        recoilZ = recZ;

        snap = snappiness;
        returnSpeed = returnSpd;
    }

    // Update is called once per frame
    void Update()
    {
        targetRot = Vector3.Lerp(targetRot, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRot = Vector3.Slerp(currentRot, targetRot, snap * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRot);

    }

    public void RecoilFire()
    {
        targetRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}