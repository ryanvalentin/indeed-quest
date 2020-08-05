using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Container for a simple dialogue popup UI item.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class InspectDialogue : MonoBehaviour
{
    public Canvas WorldSpaceCanvas;

    [Tooltip("The title of the object shown upon inspection.")]
    public string PopupTitle;

    [Tooltip("The description of the object shown upon inspection.")]
    public string PopupDescription;

    public virtual void OnInspect()
    {
        GameController.Instance.OnPopupTrigger(PopupTitle, PopupDescription);
    }

    private void OnEnable()
    {
        WorldSpaceCanvas.gameObject.SetActive(false);

        if (!WorldSpaceCanvas.worldCamera)
            WorldSpaceCanvas.worldCamera = Camera.main;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        WorldSpaceCanvas.gameObject.SetActive(true);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        WorldSpaceCanvas.gameObject.SetActive(false);
    }
}