using UnityEngine;

[CreateAssetMenu(fileName = "GunData", menuName = "Scriptable Objects/GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public float fireRate;
    public int damage;
    public int magazineSize;
    public float reloadTime;
    public Vector3 position = new Vector3(-0.134f,-0.162f,0.331f);

    public Vector3 adsPosition;
    public float adsTime;

    
}
