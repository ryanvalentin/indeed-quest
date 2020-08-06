using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the player to manage their current active quest. This should be able to
/// show the current quest to the player and allow them to cancel it even after they've
/// accepted it.
/// </summary>
public class PlayerQuestController : MonoBehaviour
{
    public static PlayerQuestController Instance { get; private set; }

    [SerializeField]
    private QuestProfile _activeQuest;

    private NPCController _originalController;

    public Canvas QuestItemCanvas;

    public Image QuestIconImage;

    public QuestProfile ActiveQuest
    {
        get { return _activeQuest; }
        set
        {
            _activeQuest = value;
            OnActiveQuestChanged();
        }
    }

    public List<QuestProfile> CompletedQuests { get; } = new List<QuestProfile>();

    public void OnStartQuest(QuestProfile quest)
    {
        ActiveQuest = quest;
    }

    public void OnCancelQuest()
    {
        ActiveQuest = null;
    }

    public void OnCompleteQuest()
    {
        CompletedQuests.Add(ActiveQuest);

        Inventory.Instance.OnItemTaken();

        ActiveQuest = null;
    }

    public void ShowActiveQuest()
    {
        if (!ActiveQuest)
            return;

        GameController.Instance.OnActiveQuestPopupTrigger(ActiveQuest.Name, ActiveQuest.InstructionsText, ActiveQuest.Owner.Profile.Icon, gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        OnActiveQuestChanged();
    }

    private void OnActiveQuestChanged()
    {
        if (!_activeQuest)
        {
            if (QuestItemCanvas.isActiveAndEnabled)
                QuestItemCanvas.enabled = false;

            QuestIconImage.sprite = null;
        }
        else
        {
            if (!QuestItemCanvas.isActiveAndEnabled)
                QuestItemCanvas.enabled = true;

            QuestIconImage.sprite = ActiveQuest.Owner.Profile.Icon;
        }
    }
}
