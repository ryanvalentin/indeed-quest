using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Container for a simple dialogue popup UI item.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class InteractableController : MonoBehaviour
{
    public Canvas WorldSpaceCanvas;

    public InteractableProfile Profile;

    public virtual void OnInteract()
    {
        GameController.Instance.OnPopupTrigger(Profile.Title, Profile.Description, Profile.Icon);
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