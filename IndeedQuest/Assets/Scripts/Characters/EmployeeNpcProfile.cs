using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an NPC's stats
/// </summary>
[CreateAssetMenu(fileName = "EmployeeProfile", menuName = "Indeed/Employee NPC", order = 1)]
public class EmployeeNpcProfile : ScriptableObject
{
    public string JobTitle;

    [Range(0, 10), Tooltip("Represents the minimum number of people this person can help get jobs, per minute.")]
    public int MinJobsPerMinute = 0;

    [Range(1, 20), Tooltip("Represents the maximum number of people this person can help get jobs, per minute.")]
    public int MaxJobsPerMinute = 10;
}
