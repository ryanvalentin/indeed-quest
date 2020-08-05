using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCollectableProfile", menuName = "Indeed/Collectable Profile", order = 1)]
public class CollectableProfile : InteractableProfile
{
    public string Id = Guid.NewGuid().ToString();
}
