using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomEvents : MonoBehaviour
{
    // Fire events
    public static event Action<InputAction.CallbackContext> OnFireWeapon;
    public static void FireWeapon(InputAction.CallbackContext context) => OnFireWeapon?.Invoke(context);

    public static event Action<FiredWeaponEnum, Transform>  OnCreateProjectile;
    public static void CreateProjectile(FiredWeaponEnum firedWeapon, Transform playerTransform) => OnCreateProjectile?.Invoke(firedWeapon, playerTransform);

    //GroundLayer Updates
    public static event Action<Vector3> OnUpdateGroundLayer;
    public static void UpdateGroundLayer(Vector3 Velocity) => OnUpdateGroundLayer?.Invoke(Velocity);

}
