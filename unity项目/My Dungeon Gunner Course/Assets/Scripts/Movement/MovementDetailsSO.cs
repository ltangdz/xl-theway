using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    #region Header MOVEMENT DETAILS

    [Space(10)]
    [Header("MOVEMENT DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random speed between min and max.")]

    #endregion
    public float minMoveSpeed = 8f;
    
    #region Tooltip

    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random speed between min and max.")]

    #endregion
    public float maxMoveSpeed = 8f;

    #region Tooltip

    [Tooltip("if there is a roll movement -- this is roll speed.")]

    #endregion
    public float rollSpeed;    // for player
    
    #region Tooltip
    
    [Tooltip("if there is a roll movement -- this is roll distance.")]

    #endregion
    public float rollDistance;   // for player
    
    #region Tooltip

    [Tooltip("if there is a roll movement -- this is roll cooldown time between roll actions.")]

    #endregion
    public float rollCooldownTime;   // for player

    public float GetMoveSpeed()
    {
        if (Mathf.Abs(minMoveSpeed - maxMoveSpeed) < 0.01f)
        {
            return minMoveSpeed;
        }
        return Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    #region Validation

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed),
            maxMoveSpeed, false);

        if (rollSpeed != 0f || rollDistance != 0f || rollCooldownTime != 0f)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldownTime), rollCooldownTime, false);
        }
    }
#endif

    #endregion
}
