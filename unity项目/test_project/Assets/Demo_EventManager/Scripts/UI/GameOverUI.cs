using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    private void Start()
    {
        Hide();
    }

    private void Awake()
    {
        EventManager.Instance.AddListener(EventName.PlayerDead, Show);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventName.PlayerDead, Show);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show(object sender, EventArgs args)
    {
        if (args is PlayerDeadEventArgs data)
        {
            Debug.Log($"{data.playerName} dead, Game over!!!");
            playerNameText.text = data.playerName + ", 你死了!!!";
        }
        gameObject.SetActive(true);
    }
}
