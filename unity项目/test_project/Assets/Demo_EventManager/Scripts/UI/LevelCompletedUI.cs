using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletedUI : MonoBehaviour
{
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        Debug.Log("你好");
        Hide();
    }

    private void Awake()
    {
        EventManager.Instance.AddListener(EventName.LevelCompleted, Show);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.LevelCompleted, Show);
    }

    private void Hide()
    {
        Debug.Log("hide");
        gameObject.SetActive(false);
    }

    private void Show(object sender, EventArgs args)
    {
        if (args is LevelCompletedEventArgs data)
        {
            Debug.Log($"{data.playerName}, level{data.levelId}: {data.levelName} completed!!!");
        }
        gameObject.SetActive(true);
    }
}
