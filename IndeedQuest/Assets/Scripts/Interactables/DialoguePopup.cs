using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    private GameObject _lastSender;

    private Text _primaryButtonText;

    private Text _secondaryButtonText;

    public Text TitleText;

    public Text DescriptionText;

    public Image IconImage;

    public Button PrimaryButton;

    public Button SecondaryButton;

    public UnityEvent OnPrimaryClick;

    public UnityEvent OnSecondaryClick;

    public void Show(GameObject sender, string title, string description, Sprite icon, string secondaryText = "Close", string primaryText = "")
    {
        _lastSender = sender;
        _primaryButtonText = PrimaryButton.GetComponentInChildren<Text>();
        _secondaryButtonText = SecondaryButton.GetComponentInChildren<Text>();

        if (gameObject.activeInHierarchy)
            return;

        IconImage.sprite = icon;

        _primaryButtonText.text = primaryText;
        PrimaryButton.gameObject.SetActive(!String.IsNullOrEmpty(primaryText));

        _secondaryButtonText.text = secondaryText;

        TitleText.text = title;
        DescriptionText.text = description;

        gameObject.SetActive(true);

        GameController.Instance.PauseGame(showMenu: false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        GameController.Instance.ResumeGame();
    }

    public void ClickPrimary()
    {
        Hide();
        OnPrimaryClick.Invoke();
        _lastSender.SendMessage("OnPrimaryDialogueButtonClick", SendMessageOptions.DontRequireReceiver);
        _lastSender = null;
    }

    public void ClickSecondary()
    {
        Hide();
        OnSecondaryClick.Invoke();
        _lastSender.SendMessage("OnSecondaryDialogueButtonClick", SendMessageOptions.DontRequireReceiver);
        _lastSender = null;
    }
}
