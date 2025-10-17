using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 静态类 保存事件名称
// 可以用enum代替 但是装箱拆箱会带来性能损耗
public static class EventName
{
    // public static string PlayerDead = "PlayerDead"; 等价
    public static string PlayerDead = nameof(PlayerDead);
    public static string LevelCompleted = nameof(LevelCompleted);
}
