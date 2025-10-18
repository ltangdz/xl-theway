using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class AimWeaponEvent : MonoBehaviour
{
    //public delegate void MyDelegate(int p1, int p2);
    //public event MyDelegate onMyDelegateEvent;

    // public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public event Action<AimWeaponEvent, AimWeaponEventArg> OnWeaponAim;

    /// <summary>
    /// 发布武器瞄准事件
    /// </summary>
    /// <param name="aimDirection">瞄准方向枚举</param>
    /// <param name="aimAngle">瞄准角度</param>
    /// <param name="weaponAimAngle">武器瞄准角度</param>
    /// <param name="weaponAimDirectionVector">武器瞄准方向向量</param>
    public void CallAimWeaponEvent(AimDirection aimDirection, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // OnWeaponAim?.Invoke(this, new AimWeaponEventArg(aimDirection, aimAngle, weaponAimAngle, weaponAimDirectionVector));
        OnWeaponAim?.Invoke(this,
            new AimWeaponEventArg()
            {
                aimDirection = aimDirection, aimAngle = aimAngle, weaponAimAngle = weaponAimAngle,
                weaponAimDirectionVector = weaponAimDirectionVector
            });
    }
}

public class AimWeaponEventArg : EventArgs
{
    public AimDirection aimDirection;
    public float aimAngle;
    public float weaponAimAngle;
    public Vector3 weaponAimDirectionVector;
}

