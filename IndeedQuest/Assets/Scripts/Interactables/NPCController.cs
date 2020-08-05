using UnityEngine;

public class NPCController : InteractableController
{
    private Renderer[] _renderers;

    private NPCProfile CharacterProfile
    {
        get { return Profile as NPCProfile; }
    }

    public void Hide()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = false;
        }
    }

    public void Show()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = true;
        }
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
        _renderers = GetComponentsInChildren<Renderer>();
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
