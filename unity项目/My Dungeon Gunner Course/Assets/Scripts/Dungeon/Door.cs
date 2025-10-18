using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES

    [Space(10)]
    [Header("OBJECT REFERENCES")]

    #endregion

    #region Tooltip

    [Tooltip("Populate this with the BoxCollider2D component on the DoorCollider game object")]

    #endregion
    [SerializeField]
    private BoxCollider2D doorCollider;
    
    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previousIsOpened = false;
    private Animator animator;

    private void Awake()
    {
        // 初始化时关闭门的碰撞 玩家进入房间战斗时打开碰撞
        doorCollider.enabled = false;
        
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        // 玩家远离房间时会禁用房间（性能原因） 再次靠近房间时会启用房间 门的动画会重置 所以我们保存门的动画状态
        animator.SetBool(Settings.open, isOpen);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Settings.playerTag) || collision.CompareTag(Settings.playerWeapon))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (!isOpen)
        {
            isOpen = true;
            previousIsOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;
            
            animator.SetBool(Settings.open, true);
            
            // 播放开门音效
            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.doorOpenCloseSoundEffect);
        }
    }

    public void LockDoor()
    {
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor()
    {
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previousIsOpened)
        {
            isOpen = false;
            OpenDoor();
        }
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }
#endif

    #endregion
}
