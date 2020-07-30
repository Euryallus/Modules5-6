using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// ## HUGO BAILEY
// ## Written: Prototype phase
// ## Purpose: Stores values associated with difficulty level
//

[CreateAssetMenu(fileName = "Difficulty Setting", menuName = "Difficulty/New Difficulty")]
public class difficultySetting : ScriptableObject
{
    //allows all values associated with difficulty to be altered by designer
    [Header("Enemy alterations")]

    [Tooltip("Suggested values between 0.5f (half base health) and 2f (two times base health)")]
    // % change on enemy health
    public float healthPercentageChange = 1f;

    [Tooltip("Suggested values between 0.5f (half base speed) and 2f (two times base speed)")]
    // % change on enemy speed
    public float speedPercentageChange = 1f;

    [Tooltip("Suggested values between 0.5f (half base damage) and 2f (two times base speed)")]
    // % change on enemy damage output
    public float damagePercentageChange = 1f;
}
