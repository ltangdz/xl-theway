using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Dead();
        }
    }

    // 玩家死亡时 通知ui 显示游戏失败信息
    public void Dead()
    {
        Debug.Log("player dead!!!");
        // 发布死亡事件
        this.TriggerEvent(EventName.PlayerDead, new PlayerDeadEventArgs{playerName = gameObject.name});
    }
}
