using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponInput : MonoBehaviour
{
    public Animator animator;
    public PlayerMovementInput playerMovement;
    EquippedWeaponEnum equippedWeapon;
    public Weapon[] weapons = new Weapon[3];

    void Awake()
    {
        equippedWeapon = EquippedWeaponEnum.knifeEquipped;
        animator.SetBool(equippedWeapon.ToString(), true);

        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.ChangeWeapon.performed += ChangeWeapon_performed;
        playerInputActions.Player.Fire.performed += FiredWeapon_performed;
        playerInputActions.Player.Fire.canceled += FiredWeapon_Canceled;
    }

    private void ChangeWeapon_performed(InputAction.CallbackContext context)
    {
        if (int.TryParse(context.control.name, out int weaponIndex))
        {
            int adjustedWeaponIndex = weaponIndex - 1;   
            if (weapons[adjustedWeaponIndex].enabled)
            {
                equippedWeapon = (EquippedWeaponEnum)adjustedWeaponIndex;
                foreach (EquippedWeaponEnum weapon in Enum.GetValues(typeof(EquippedWeaponEnum)))
                {
                    animator.SetBool(weapon.ToString(), weapon == equippedWeapon);
                }
            }
        }
    }

    private void FiredWeapon_performed(InputAction.CallbackContext context)
    {
        int ammo = weapons[(int)equippedWeapon].ammo;

        if(ammo > 0 || ammo < 0)
        {
            
            CustomEvents.FireWeapon(context);

            //Casts the equippedWeapon into a firedWeapon Enum which then sets of the trigger.
            FiredWeaponEnum firedWeapon = (FiredWeaponEnum)(int)equippedWeapon;
            animator.SetTrigger(firedWeapon.ToString());
            weapons[(int)equippedWeapon].DecreaseAmmo(firedWeapon, transform);


        }


    }

    private void FiredWeapon_Canceled(InputAction.CallbackContext context)
    {
        FiredWeaponEnum firedWeapon = (FiredWeaponEnum)(int)equippedWeapon;
        animator.ResetTrigger(firedWeapon.ToString());
    }
}
