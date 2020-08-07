using UnityEngine;

public class CollectibleController : InteractableController
{
    public bool IsItemTaken { get; private set; } = false;

    public CollectibleProfile CollectibleProfile
    {
        get { return Profile as CollectibleProfile; }
    }

    public void TakeItem()
    {
        IsItemTaken = true;

        gameObject.SetActive(false);

        Inventory.Instance.OnReceiveItem(this);
    }

    public void ReturnItem()
    {
        IsItemTaken = false;

        gameObject.SetActive(true);
    }

    public override void OnInteract()
    {
        GameController.Instance.OnCollectiblePopupTrigger(Profile.Title, CollectibleProfile.Description, Profile.Icon, gameObject);
    }

    public void OnPrimaryDialogueButtonClick()
    {
        TakeItem();
    }

    protected override void Start()
    {
        base.Start();

        GameController.Instance.RegisterCollectible(this);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Profile != default && CollectibleProfile is null)
            Debug.LogError($"Profile for controller {name}({nameof(CollectibleController)}) needs to be of type {nameof(CollectibleProfile)}");
    }
#endif
}
