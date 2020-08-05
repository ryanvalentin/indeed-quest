using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an NPC's stats
/// </summary>
[CreateAssetMenu(fileName = "NewNPCProfile", menuName = "Indeed/NPC Profile", order = 1)]
public class NPCProfile : InteractableProfile
{
    [Header("Productivity")]

    [Range(0, 10), Tooltip("Represents the minimum number of people this person can help get jobs, per minute.")]
    public int MinJobsPerMinute = 0;

    [Range(1, 20), Tooltip("Represents the maximum number of people this person can help get jobs, per minute.")]
    public int MaxJobsPerMinute = 10;

    [Header("Dialogue")]

    [Tooltip("Range of things this NPC will say when idle.")]
    public Dialogue[] IdleDialogue;

    [Serializable]
    public class Dialogue
    {
        [Tooltip("The actual body of text.")]
        public string Text;
    }
}
