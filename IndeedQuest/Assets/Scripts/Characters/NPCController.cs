using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : InspectDialogue
{
    private Renderer[] _renderers;

    public EmployeeNpcProfile Profile;

    public bool HasQuest = true;

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
        if (Profile)
            PopupTitle = Profile.JobTitle;
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
