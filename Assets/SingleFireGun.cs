using UnityEngine;

public class SingleFireGun : Gun
{
    public override void Shoot()
    {

        if (!CanShoot()) return;

        if (shootTimer < gunData.fireRate)
        {
            shootTimer += Time.deltaTime;
        }

        if (Input.GetButtonDown("Fire1") && shootTimer >= gunData.fireRate && ammo > 0)
        {

            base.Raycast();
            base.Recoil();
            if (anim)
            {
                anim.SetTrigger("shoot");
            }
            ammo--;
            GetPlayerInterfaceManager().UpdateAmmoText(ammo);
            shootTimer -= gunData.fireRate;
        }
    }
}

