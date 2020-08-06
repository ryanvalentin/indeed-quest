using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField]
    private CollectibleProfile _currentItem;

    private CollectibleController _originalController;

    public CollectibleProfile CurrentItem
    {
        get { return _currentItem; }
        private set
        {
            _currentItem = value;
            OnCurrentItemChange();
        }
    }

    public Canvas InventoryItemCanvas;

    public Image InventoryIconImage;

    // Start is called before the first frame update
    private void Start()
    {
        Instance = this;
        OnCurrentItemChange();
    }

    public void ShowCurrentItem()
    {
        if (!CurrentItem)
            return;

        GameController.Instance.OnInventoryTrigger(CurrentItem.Title, CurrentItem.Description, CurrentItem.Icon, gameObject);
    }

    public void OnPrimaryDialogueButtonClick()
    {
        // This means the player wants to drop the item.
        _originalController.ReturnItem();
        CurrentItem = null;
        _originalController = null;
    }

    public void OnReceiveItem(CollectibleController collectible)
    {
        _originalController = collectible;
        CurrentItem = collectible.CollectibleProfile;
    }

    public void OnItemTaken()
    {
        // Item has been taken, so destroy the original
        Destroy(_originalController.gameObject);
        CurrentItem = null;
        _originalController = null;
    }

    private void OnCurrentItemChange()
    {
        if (!_currentItem)
        {
            if (InventoryItemCanvas.isActiveAndEnabled)
                InventoryItemCanvas.enabled = false;

            InventoryIconImage.sprite = null;
        }
        else
        {
            if (!InventoryItemCanvas.isActiveAndEnabled)
                InventoryItemCanvas.enabled = true;

            InventoryIconImage.sprite = CurrentItem.Icon;
        }
    }
}
