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

    public Button PrimaryButton;

    public Button SecondaryButton;

    public UnityEvent OnPrimaryClick;

    public UnityEvent OnSecondaryClick;

    // Start is called before the first frame update
    private void Start()
    {
        _primaryButtonText = PrimaryButton.GetComponentInChildren<Text>();
        _secondaryButtonText = SecondaryButton.GetComponentInChildren<Text>();
    }

    public void Show(string title, string description, string secondaryText = "Close", string primaryText = "")
    {
        if (gameObject.activeInHierarchy)
            return;

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
