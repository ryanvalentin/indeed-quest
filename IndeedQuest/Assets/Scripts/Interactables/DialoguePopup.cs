using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    private Text _primaryButtonText;

    private Text _secondaryButtonText;

    public Text TitleText;

    public Text DescriptionText;

    public Image IconImage;

    public Button PrimaryButton;

    public Button SecondaryButton;

    public UnityEvent OnPrimaryClick;

    public UnityEvent OnSecondaryClick;

    public void Show(string title, string description, Sprite icon, string secondaryText = "Close", string primaryText = "")
    {
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
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ClickPrimary()
    {
        Hide();
        OnPrimaryClick.Invoke();
    }

    public void ClickSecondary()
    {
        Hide();
        OnSecondaryClick.Invoke();
    }
}
