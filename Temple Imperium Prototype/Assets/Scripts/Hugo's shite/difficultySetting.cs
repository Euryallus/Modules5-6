using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Difficulty Setting", menuName = "Difficulty/New Difficulty")]
public class difficultySetting : ScriptableObject
{
    [Header("Enemy alterations")]

    [Tooltip("Suggested values between 0.5f (half base health) and 2f (two times base health)")]
    public float healthPercentageChange = 1f;

    [Tooltip("Suggested values between 0.5f (half base speed) and 2f (two times base speed)")]
    public float speedPercentageChange = 1f;

    [Tooltip("Suggested values between 0.5f (half base damage) and 2f (two times base speed)")]
    public float damagePercentageChange = 1f;
}
