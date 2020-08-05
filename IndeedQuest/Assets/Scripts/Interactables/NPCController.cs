using UnityEngine;

public class NPCController : InteractableController
{
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
    }

    // Update is called once per frame
    private void Update()
    {
        
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
