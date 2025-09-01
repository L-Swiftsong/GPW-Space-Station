using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccessibilityText : ProtectedSingleton<AccessibilityText>
{
    private const PlayerInput.ActionTypes PREVENTED_INPUT_TYPES = PlayerInput.ActionTypes.Everything & ~PlayerInput.ActionTypes.Interaction;

    [Header("With Header Text")]
    [SerializeField] private GameObject _withHeaderContainer;
    [SerializeField] private TMP_Text _bodyWithContainerText;
    [SerializeField] private TMP_Text _titleText;


    [Header("Without Header Text")]
    [SerializeField] private GameObject _withoutHeaderContainer;
    [SerializeField] private TMP_Text _bodyWithoutContainerText;



    protected override void Awake()
    {
        base.Awake();
        StopDisplayingText();
    }
    private void OnDestroy()
    {
        // For saftey, remove action prevention when destroyed in case it was open when the player dies or something.
        PlayerInput.RemoveActionPrevention(typeof(AccessibilityText), PREVENTED_INPUT_TYPES);
    }


    public static void DisplayAccessibleText(string text, string titleText = null)
    {
        if (AccessibilityText.HasInstance)
        {
            if (titleText == null)
                AccessibilityText.Instance.DisplayText(text);
            else
                AccessibilityText.Instance.DisplayText(text, titleText);
            
            PlayerInput.PreventActions(typeof(AccessibilityText), PREVENTED_INPUT_TYPES);
        }
    }
    public static void StopDisplayingAccessibleText()
    {
        if (AccessibilityText.HasInstance)
        {
            AccessibilityText.Instance.StopDisplayingText();
            PlayerInput.RemoveActionPrevention(typeof(AccessibilityText), PREVENTED_INPUT_TYPES);
        }
    }
    private void DisplayText(string text)
    {
        this.gameObject.SetActive(true);
        this._withHeaderContainer.SetActive(false);
        this._withoutHeaderContainer.SetActive(true);

        // Set the text.
        _bodyWithoutContainerText.text = text;
    }
    private void DisplayText(string text, string titleText)
    {
        this.gameObject.SetActive(true);
        this._withHeaderContainer.SetActive(true);
        this._withoutHeaderContainer.SetActive(false);

        // Set the text.
        _titleText.text = titleText;
        _bodyWithContainerText.text = text;
    }
    private void StopDisplayingText()
    {
        this.gameObject.SetActive(false);
    }
}
