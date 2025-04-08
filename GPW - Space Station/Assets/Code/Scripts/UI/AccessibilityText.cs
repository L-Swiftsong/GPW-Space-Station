using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AccessibilityText : ProtectedSingleton<AccessibilityText>
{
    [SerializeField] private TMP_Text _text;
    private const PlayerInput.ActionTypes PREVENTED_INPUT_TYPES = PlayerInput.ActionTypes.Everything & ~PlayerInput.ActionTypes.Interaction;


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


    public static void DisplayAccessibleText(string text)
    {
        if (AccessibilityText.HasInstance)
        {
            AccessibilityText.Instance.DisplayText(text);
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
        _text.text = text;
    }
    private void StopDisplayingText()
    {
        this.gameObject.SetActive(false);
    }
}
