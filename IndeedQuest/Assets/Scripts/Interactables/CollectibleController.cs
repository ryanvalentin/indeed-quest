using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollectibleController : InteractableController
{
    public bool IsItemTaken { get; private set; } = false;

    private CollectibleProfile CollectibleProfile
    {
        get { return Profile as CollectibleProfile; }
    }

    public void TakeItem()
    {
        IsItemTaken = true;

        // TODO: Hide this item

        Inventory.Instance.CurrentItem = CollectibleProfile;
    }

    public override void OnInteract()
    {
        GameController.Instance.OnCollectiblePopupTrigger(Profile.Title, CollectibleProfile.Description, Profile.Icon, gameObject);
    }

    public void OnPrimaryDialogueButtonClick()
    {
        TakeItem();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Profile != default && CollectibleProfile is null)
            Debug.LogError($"Profile for controller {name}({nameof(CollectibleController)}) needs to be of type {nameof(CollectibleProfile)}");
    }
#endif
}
