using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonobehavior<GameManager>
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    [SerializeField] private int levelId = 1;
    [SerializeField] private string levelName = "极寒之地";
    [SerializeField] private string playerName = "狗蛋";

    public int LevelId => levelId;

    public string LevelName => levelName;

    public string PlayerName => playerName;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            LevelCompleted();
        }
    }

    private void LevelCompleted()
    {
        Debug.Log("通关！！！");
        this.TriggerEvent(EventName.LevelCompleted, new LevelCompletedEventArgs{levelId = levelId, playerName = playerName, levelName = levelName});
    }
}
