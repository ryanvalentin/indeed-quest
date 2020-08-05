using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
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
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasQuest)
            return;

        Debug.Log($"Interacted with {name} - {Profile.JobTitle}");
    }
}
