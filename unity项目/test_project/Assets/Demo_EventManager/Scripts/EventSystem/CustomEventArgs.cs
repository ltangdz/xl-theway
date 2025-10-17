using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 自定义事件类
public class PlayerDeadEventArgs : EventArgs
{
    public string playerName;
}

public class LevelCompletedEventArgs : EventArgs
{
    public int levelId;
    public string levelName;
    public string playerName;
}