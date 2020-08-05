using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : InteractableController
{
    private float _currentProductivity;

    [Min(0f), Tooltip("The minimum amount of time before calculating new jobs for the score.")]
    public float ReportingIntervalMinimum = 2f;

    [Min(1f), Tooltip("The maximum amount of time before calculating new jobs for the score.")]
    public float ReportingIntervalMaxiumum = 10f;

    private NPCProfile CharacterProfile
    {
        get { return Profile as NPCProfile; }
    }

    public override void OnInteract()
    {
        // TODO: If this NPC has a quest, use a different dialogue.

        var dialogue = CharacterProfile.IdleDialogue[Random.Range(0, CharacterProfile.IdleDialogue.Length - 1)];

        GameController.Instance.OnPopupTrigger(Profile.Title, dialogue.Text, Profile.Icon);
    }

    // Start is called before the first frame update
    private void Start()
    {
        GameController.Instance.RegisterNPC(this);
        StartCoroutine(RunContributeScoreRoutine());
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateQuests();
        UpdateProductivity();
    }

    private void UpdateProductivity()
    {
        // TODO: reduce this based on time until next quest (and 0 if they have a quest)
        _currentProductivity = 1f;
    }

    private IEnumerator RunContributeScoreRoutine()
    {
        // For the length of the game, report the number of jobs this person helped find.

        while (true)
        {
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
