using UnityEngine;

public class AutomaticGun : Gun
{
    public override void Shoot()
    {
        if(shootTimer < gunData.fireRate)
        {
            shootTimer += Time.deltaTime;
        }
        
        if(Input.GetButton("Fire1") && shootTimer >= gunData.fireRate)
        {
            
            base.Raycast();
            base.Recoil();
            shootTimer -= gunData.fireRate;
        }
    }
}
