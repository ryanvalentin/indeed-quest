using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a prototype of a quest.
/// </summary>
[CreateAssetMenu(fileName = "NewQuestProfile", menuName = "Indeed/Quest Profile", order = 1)]
public class QuestProfile : ScriptableObject
{
    [Tooltip("The name of the quest.")]
    public string Name;

    [TextArea(2, 8), Tooltip("What the NPC says before the quest is accepted.")]
    public string InstructionsText;

    [TextArea(2, 8), Tooltip("What the NPC says if you bring them the wrong thing.")]
    public string WrongItemText;

    [TextArea(2, 8), Tooltip("Random things the NPC can say when you talk to them without an item.")]
    public string[] QuestIncompleteText;

    [TextArea(2, 8), Tooltip("What the NPC says when you complete the quest.")]
    public string CompleteText;

    [Tooltip("Whether to give a random start time to this quest.")]
    public bool RandomizeStartTime = false;

    [Range(0f, 28799f), Tooltip("Time in seconds since the start of the day that this quest is triggered")]
    public float StartTime;

    [Tooltip("The collectible requested in this quest.")]
    public CollectibleProfile Collectible;

    [HideInInspector]
    public bool IsComplete = false;

    [HideInInspector]
    public NPCController Owner;
}
