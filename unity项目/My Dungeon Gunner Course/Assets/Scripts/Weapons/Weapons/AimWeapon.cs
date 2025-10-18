using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AimWeaponEvent))]
[DisallowMultipleComponent]
public class AimWeapon : MonoBehaviour
{
    #region Tooltip

    [Tooltip("Populate with the Transform from the child WeaponRotationPoint gameObject")]

    #endregion

    [SerializeField]
    private Transform weaponRotationPointTransform;

    private AimWeaponEvent aimWeaponEvent;

    private void Awake()
    {
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
    }

    private void OnEnable()
    {
        aimWeaponEvent.OnWeaponAim += AimWeaponEvent_OnWeaponAim;
    }

    private void OnDisable()
    {
        aimWeaponEvent.OnWeaponAim -= AimWeaponEvent_OnWeaponAim;
    }

    private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArg aimWeaponEventArg)
    {
        Aim(aimWeaponEventArg.aimDirection, aimWeaponEventArg.aimAngle);
    }

    /// <summary>
    /// 武器瞄准
    /// </summary>
    /// <param name="aimDirection"></param>
    /// <param name="aimAngle"></param>
    private void Aim(AimDirection aimDirection, float aimAngle)
    {
        weaponRotationPointTransform.eulerAngles = new Vector3(0, 0, aimAngle);

        switch (aimDirection)
        {
            case AimDirection.Left:
            case AimDirection.UpLeft:
                weaponRotationPointTransform.localScale = new Vector3(1f, -1f, 0);
                break;
            
            case AimDirection.Up:
            case AimDirection.Down:
            case AimDirection.Right:
            case AimDirection.UpRight:
                weaponRotationPointTransform.localScale = new Vector3(1f, 1f, 0);
                break;
                
            default:
                break;
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRotationPointTransform), weaponRotationPointTransform);
    }
#endif

    #endregion
}














