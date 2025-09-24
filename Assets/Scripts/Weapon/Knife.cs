using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Weapon
{

    void Awake()
    {
        damage = 10;
        ammo   = -10;
        this.enabled = true;
    }

}
