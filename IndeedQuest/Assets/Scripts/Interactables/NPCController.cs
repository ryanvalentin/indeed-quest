using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : InteractableController
{
    private float _currentProductivity;

    public QuestProfile Quest = null;

    [Min(0f), Tooltip("The minimum amount of time before calculating new jobs for the score.")]
    public float ReportingIntervalMinimum = 2f;

    [Min(1f), Tooltip("The maximum amount of time before calculating new jobs for the score.")]
    public float ReportingIntervalMaxiumum = 10f;

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

            return GameController.Instance.TimeSinceStart >= Quest.StartTime; 
        }
    }

    public bool IsQuestActive
    {
        get
        {
            return Quest != null && GameController.Instance.ActiveQuest == Quest;
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
            else if (Inventory.Instance.CurrentItem.Id == Quest.Collectible.Id)
            {
                GameController.Instance.OnPopupTrigger(Profile.Title, Quest.CompleteText, Profile.Icon, gameObject);
                GameController.Instance.OnCompleteQuest();
            }
            else
            {
                GameController.Instance.OnPopupTrigger(Profile.Title, Quest.WrongItemText, Profile.Icon, gameObject);
            }
        }
        else if (HasQuest)
        {
            GameController.Instance.OnQuestPopupTrigger(Profile.Title, Quest.InstructionsText, Profile.Icon, gameObject);
            GameController.Instance.OnStartQuest(Quest);
        }
        else
        {
            var dialogue = CharacterProfile.IdleDialogue[Random.Range(0, CharacterProfile.IdleDialogue.Length)];

            GameController.Instance.OnPopupTrigger(Profile.Title, dialogue.Text, Profile.Icon, gameObject);
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
}
