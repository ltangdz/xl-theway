using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Health类不仅用于玩家 还可以用于其他需要Health的类
/// </summary>
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;

    public void SetStartingHealth(int startingHealth)
    {
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth()
    {
        return startingHealth;
    }
}
