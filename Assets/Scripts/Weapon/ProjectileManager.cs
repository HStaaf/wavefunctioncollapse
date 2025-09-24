using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;

    private void OnEnable()
    {
        CustomEvents.OnCreateProjectile += CreateProjectile;
    }

    private void OnDisable()
    {
        CustomEvents.OnCreateProjectile -= CreateProjectile;
    }

    private void CreateProjectile(FiredWeaponEnum firedWeapon, Transform playerTransform)
    {

        Vector3 positionAdjustment = Vector3.zero;

        switch (firedWeapon)
        {
            case FiredWeaponEnum.firingGun:
                positionAdjustment = -playerTransform.up * 4f - playerTransform.right * 0.6f;
                break;
            case FiredWeaponEnum.firingRifle:
                // Adjust as needed for the rifle
                positionAdjustment = -playerTransform.up * 8f - playerTransform.right * 0.44f;
                break;
        }

        // Instantiate the bullet as a child of the ProjectileManager GameObject
        Instantiate(bulletPrefab, playerTransform.position + positionAdjustment, playerTransform.rotation, this.transform);
    }
}
