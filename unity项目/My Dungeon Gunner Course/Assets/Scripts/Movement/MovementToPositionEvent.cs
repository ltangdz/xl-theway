using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementToPositionEvent : MonoBehaviour
{
    public event Action<MovementToPositionEvent, MovementToPositionArgs> OnMovementToPosition;

    public void CallMovementToPosition(Vector3 movePosition, Vector3 currentPosition, float moveSpeed,
        Vector3 moveDirection, bool isRolling)

    {
        OnMovementToPosition?.Invoke(this,
            new MovementToPositionArgs()
            {
                movePosition = movePosition, currentPosition = currentPosition, moveSpeed = moveSpeed,
                moveDirection = moveDirection, isRolling = isRolling
            });
    }
}

public class MovementToPositionArgs : EventArgs
{
    // 目标坐标
    public Vector3 movePosition;
    
    // 当前坐标
    public Vector3 currentPosition;
    
    // 翻滚速度
    public float moveSpeed;
    
    // 翻滚方向
    public Vector3 moveDirection;
    
    // 正在翻滚？
    public bool isRolling;
}
