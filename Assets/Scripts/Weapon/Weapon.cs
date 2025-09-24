using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int damage;
    public int ammo; 

    public void DecreaseAmmo(FiredWeaponEnum firedWeapon, Transform playerTransform) 
    {
        if (ammo > 0)
        {
            ammo--;
            CustomEvents.CreateProjectile(firedWeapon, playerTransform);
        }

    }
}
