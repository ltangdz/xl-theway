using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle事件的subscriber player身上有Idle组件 代表player会监听Idle事件
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private IdleEvent idleEvent;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        idleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    private void MoveRigidBody()
    {
        rigidbody2D.velocity = Vector2.zero;
    }
}
