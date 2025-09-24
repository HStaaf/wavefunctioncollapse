using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    void Awake()
    {
        ammo = 50;
        damage = 15;
        this.enabled = true;
    }
}
