using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Container for a simple dialogue popup UI item.
/// </summary>
[RequireComponent(typeof(Collider))]
public class InteractableController : MonoBehaviour
{
    protected bool _playerIsColliding = false;
    protected bool _showAlertPermanently = false;
    protected Button _interactButton;

    public Canvas WorldSpaceCanvas;

    public InteractableProfile Profile;

    [Tooltip("The image where the interact icon appears.")]
    public Image InteractImage;

    public virtual void OnInteract()
    {
        GameController.Instance.OnPopupTrigger(Profile.Title, Profile.Description, Profile.Icon, gameObject);
    }

    private void OnEnable()
    {
        WorldSpaceCanvas.enabled = false;

        if (!WorldSpaceCanvas.worldCamera)
            WorldSpaceCanvas.worldCamera = Camera.main;
    }

    protected virtual void Start()
    {
        OnUpdateInteractButton();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsColliding = true;

            OnUpdateInteractButton();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerIsColliding = false;

            OnUpdateInteractButton();
        }
    }

    protected virtual Sprite GetInteractionSprite()
    {
        return null;
    }

    protected void OnUpdateInteractButton()
    {
        _interactButton = WorldSpaceCanvas.GetComponentInChildren<Button>();
        var imageColor = InteractImage.color;
        var sprite = GetInteractionSprite();

        if (sprite && InteractImage)
            InteractImage.sprite = sprite;

        WorldSpaceCanvas.enabled = _playerIsColliding || _showAlertPermanently;

        if (_interactButton)
            _interactButton.interactable = _playerIsColliding;

        InteractImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, _playerIsColliding ? 1f : 0.5f);

    }
}