using UnityEngine;

public class AutomaticGun : Gun
{
    public override void Shoot()
    {
        if(shootTimer < gunData.fireRate)
        {
            shootTimer += Time.deltaTime;
        }
        
        if(Input.GetButton("Fire1") && shootTimer >= gunData.fireRate && ammo > 0)
        {
            
            base.Raycast();
            base.Recoil();
            anim.SetTrigger("shoot");
            ammo--;
            GetPlayerInterfaceManager().UpdateAmmoText(ammo);
            shootTimer -= gunData.fireRate;
        }
    }
}
