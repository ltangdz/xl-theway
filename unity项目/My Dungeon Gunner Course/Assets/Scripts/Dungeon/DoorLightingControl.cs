using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家接近时渐变门
/// </summary>
[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    public void FadeInDoor(Door door)
    {
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            // 获取所有门的SpriteRenderer
            SpriteRenderer[] spriteRendererArray = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
            {
                StartCoroutine(FadeInDoorCoroutine(spriteRenderer, material));
            }
        }
        
        isLit = true;
    }

    private IEnumerator FadeInDoorCoroutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        // 设置房门渐变
        for (float i = 0.05f; i <= 1f; i += Time.fixedDeltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Silder", i);
            
            yield return null;
        }
        
        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag))
        {
            FadeInDoor(door);
        }
    }
}
