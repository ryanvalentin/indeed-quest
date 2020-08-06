using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectibleProfile", menuName = "Indeed/Collectible Profile", order = 1)]
public class CollectibleProfile : InteractableProfile
{
    public string Id = Guid.NewGuid().ToString();
}
