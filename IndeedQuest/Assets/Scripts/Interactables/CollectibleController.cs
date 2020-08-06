using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollectibleController : InteractableController
{
    private CollectibleProfile CollectibleProfile
    {
        get { return Profile as CollectibleProfile; }
    }

    public override void OnInteract()
    {
        GameController.Instance.OnPopupTrigger(Profile.Title, CollectibleProfile.Description, Profile.Icon);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Profile != default && CollectibleProfile is null)
            Debug.LogError($"Profile for controller {name}({nameof(CollectibleController)}) needs to be of type {nameof(CollectibleProfile)}");
    }
#endif
}
