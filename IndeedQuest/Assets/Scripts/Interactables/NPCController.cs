using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : InteractableController
{
    public enum NPCState
    {
        Idle,
        HasQuest,
        AwaitingQuest,
    }

    private float _currentProductivity;

    public QuestProfile Quest = null;

    [Min(0f), Tooltip("The minimum amount of time before calculating new jobs for the score.")]
    public float ReportingIntervalMinimum = 2f;

    [Min(1f), Tooltip("The maximum amount of time before calculating new jobs for the score.")]
    public float ReportingIntervalMaxiumum = 10f;

    public NPCState CurrentState
    {
        get
        {
            if (IsQuestActive)
                return NPCState.AwaitingQuest;
            else if (HasQuest)
                return NPCState.HasQuest;

            return NPCState.Idle;
        }
    }

    private NPCProfile CharacterProfile
    {
        get { return Profile as NPCProfile; }
    }

    public bool HasQuest
    {
        get 
        {
            if (Quest == null)
                return false;

            if (Quest.IsComplete)
                return false;

            return GameController.Instance.ScaledGameTime >= Quest.StartTime + GameController.START_TIME; 
        }
    }

    public bool IsQuestActive
    {
        get
        {
            return Quest != null && PlayerQuestController.Instance.ActiveQuest == Quest;
        }
    }

    public override void OnInteract()
    {
        if (IsQuestActive)
        {
            if (Inventory.Instance.CurrentItem == null)
            {
                GameController.Instance.OnPopupTrigger(Profile.Title, Quest.QuestIncompleteText[Random.Range(0, Quest.QuestIncompleteText.Length)], Profile.Icon, gameObject);
            }
            else if (Inventory.Instance.CurrentItem.Title == Quest.Collectible.Title)
            {
                GameController.Instance.OnPopupTrigger(Profile.Title, Quest.CompleteText, Profile.Icon, gameObject);
                PlayerQuestController.Instance.OnCompleteQuest();
                Quest.IsComplete = true;
                _showAlertPermanently = false;
            }
            else
            {
                GameController.Instance.OnPopupTrigger(Profile.Title, Quest.WrongItemText, Profile.Icon, gameObject);
            }
        }
        else if (HasQuest)
        {
            GameController.Instance.OnQuestPopupTrigger(Profile.Title, Quest.InstructionsText, Profile.Icon, gameObject);

            // If the user accepts the quest, it'll be sent as a message to be received at OnPrimaryDialogueButtonClick();
        }
        else
        {
            var dialogue = CharacterProfile.IdleDialogue[Random.Range(0, CharacterProfile.IdleDialogue.Length)];

            GameController.Instance.OnPopupTrigger(Profile.Title, dialogue.Text, Profile.Icon, gameObject);
        }
    }

    public void OnPrimaryDialogueButtonClick()
    {
        if (HasQuest)
        {
            // Player has accepted quest
            PlayerQuestController.Instance.OnStartQuest(Quest);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Clone the Quest so we don't modify original file in play mode.
        if (Quest != null)
        {
            Quest = Instantiate(Quest);
            Quest.IsComplete = false;
            Quest.Owner = this;
        }

        GameController.Instance.RegisterNPC(this);
        StartCoroutine(RunContributeScoreRoutine());
    }

    private IEnumerator RunContributeScoreRoutine()
    {
        // For the length of the game, report the number of jobs this person helped find.

        while (true)
        {
            if (HasQuest && !WorldSpaceCanvas.isActiveAndEnabled)
            {
                _showAlertPermanently = true;

                // If the NPC has a quest, make their bubble permanent. This won't make the button active, it'll require a trigger.
                OnUpdateInteractButton();
            }

            if (HasQuest && !IsQuestActive)
                _currentProductivity = Random.Range(0f, 0.1f);
            else
                _currentProductivity = Random.Range(0.5f, 1f); // TODO: Count productivity down the closer they are to needing something.

            var jobsContributed = Random.Range(CharacterProfile.MinJobsPerMinute, CharacterProfile.MaxJobsPerMinute) * _currentProductivity;

            GameController.Instance.OnContributeToScore((int)jobsContributed);

            float nextReportTime = Random.Range(ReportingIntervalMinimum, ReportingIntervalMaxiumum);
            yield return new WaitForSeconds(nextReportTime);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Profile != default && CharacterProfile is null)
            Debug.LogError($"Profile for controller {name}({nameof(NPCController)}) needs to be of type {nameof(NPCProfile)}");
    }
#endif

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }

    protected override Sprite GetInteractionSprite()
    {
        switch (CurrentState)
        {
            case NPCState.AwaitingQuest:
                return GameController.Instance.Profile.NPCAwaitingQuestSprite;
            case NPCState.HasQuest:
                return GameController.Instance.Profile.NPCHasQuestSprite;
            case NPCState.Idle:
            default:
                return GameController.Instance.Profile.NPCIdleSprite;

        }
    }
}
