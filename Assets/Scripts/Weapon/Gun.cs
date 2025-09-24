using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{

    void Awake()
    {
        damage = 10;
        ammo = 50;
        this.enabled = true;
    }

}
