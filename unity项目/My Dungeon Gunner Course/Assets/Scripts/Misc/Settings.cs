using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    #region UNITS
    
    public const float pixelsPerUnit = 16f;     // 一个网格单元有多少像素
    public const float tileSizePixels = 16f;    // 一个tile有多少像素
    
    #endregion
    
    #region DUNGEON BUILD SETTINGS

    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttemmpts = 10;

    #endregion
    
    #region ROOM SETTINGS

    public const float fadeInTime = 0.5f;   // 房间的淡入时间
    public const int maxChildCorridors = 3;

    #endregion

    #region ANIMATIOR PARAMETERS
    // Animator parameters -- Player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static float baseSpeedForPlayerAnimations = 8f;

    // Animator parameters -- Door
    public static int open = Animator.StringToHash("open");
    #endregion

    #region GAMEOBJECT TAGS

    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    #endregion

    #region FIRING CONTROL

    public const float useAimAngleDistance = 3.5f;  // if the target distance is less than this then the aim angle will be used (calculated from player)

    #endregion

    #region UI PARAMETERS

    public const float uiAmmoIconSpacing = 4f;

    #endregion
}
