using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to the player to manage their current active quest. This should be able to
/// show the current quest to the player and allow them to cancel it even after they've
/// accepted it.
/// </summary>
public class PlayerQuestController : MonoBehaviour
{
    [SerializeField]
    private QuestProfile _currentQuest;

    public QuestProfile CurrentQuest
    {
        get { return _currentQuest; }
        set
        {
            _currentQuest = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCurrentQuest()
    {
        if (!CurrentQuest)
            return;

        // TODO: Show the UI here
    }
}
