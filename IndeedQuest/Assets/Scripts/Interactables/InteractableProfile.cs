using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteractableProfile", menuName = "Indeed/Interactable Profile", order = 1)]
public class InteractableProfile : ScriptableObject
{
    [Header("Common")]

    [Tooltip("The name of this interactable.")]
    public string Title;

    [TextArea(2, 4), Tooltip("The description of this interactable.")]
    public string Description;

    [Tooltip("The icon associated with this interactable.")]
    public Sprite Icon;
}
