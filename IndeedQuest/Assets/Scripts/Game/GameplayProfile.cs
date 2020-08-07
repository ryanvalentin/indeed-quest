using System;
using UnityEngine;
using UnityEngine.Audio;
using static ComparisonExtensions;

/// <summary>
/// Defines the data file for gameplay parameters and conditions.
/// </summary>
[CreateAssetMenu(fileName = "NewGameplayProfile", menuName = "Indeed/Gameplay Profile", order = 1)]
public class GameplayProfile : ScriptableObject
{
    [Header("Music")]

    [Tooltip("The audio mixer to use. You can create a custom mixer to apply random sound effects.")]
    public AudioMixer Mixer;

    [Tooltip("The main music loop to play during the game.")]
    public AudioClip MusicLoop;

    [Header("Time")]

    [Range(1f, 120f), Tooltip("The multiplier the determines how fast the game runs. With a value of 1, the game will last 8 hours. If 60, it'll last 8 minutes")]
    public float TimeScale = 60f;

    [Range(0.1f, 5f), Tooltip("How long it takes to fade in/out of black screen when starting/ending the game.")]
    public float GameFadeTimeSeconds = 1f;

    [Range(0.1f, 5f), Tooltip("How long it takes to fade in/out of black screen when going between rooms.")]
    public float SceneTransitionTimeSeconds = 0.2f;

    [Header("Player")]

    [Tooltip("The name of the player.")]
    public string PlayerName = "Jobby";

    [Tooltip("The avatar for the given player.")]
    public Sprite PlayerAvatar;

    [Range(1f, 6f), Tooltip("How fast the player can move throughout the level")]
    public float PlayerMoveSpeed = 2f;

    [Header("Quests")]

    [Tooltip("Pool of quests which are assigned randomly to NPCs without a specific quest.")]
    public QuestProfile[] RandomQuests;

    public Sprite NPCIdleSprite;

    public Sprite NPCHasQuestSprite;

    public Sprite NPCAwaitingQuestSprite;

    [Header("Narrations")]

    [TextArea(2, 5), Tooltip("The text shown at the beginning of the game.")]
    public string IntroductionText;

    [Tooltip("List of endings. Executed in order of the list, so if two conditions match the earlier one takes precedence.")]
    public EndResult[] EndResults;

    [TextArea(2, 5), Tooltip("The text shown at the end of the game if no other conditions were satisfied.")]
    public string DefaultGameOverText = "What a first day! I hope we did alright...";

    [Serializable]
    public class EndResult
    {
        [Tooltip("Condition the score must be in.")]
        public EndConditionInt ScoreCondition;

        [TextArea(2, 5), Tooltip("The text shown when the game is over for this condition.")]
        public string GameOverText;
    }

    [Serializable]
    public class ComparisonCondition
    {
        public ConditionComparison Comparison;
    }

    [Serializable]
    public class EndConditionInt : ComparisonCondition
    {
        public int Value;
    }

    [Serializable]
    public class EndConditionFloat : ComparisonCondition
    {
        public float Value;
    }
}
