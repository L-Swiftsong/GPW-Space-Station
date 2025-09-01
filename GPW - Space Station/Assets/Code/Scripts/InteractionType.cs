using System.Collections.Generic;
using UI.Icons;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public enum InteractionType
{
    DefaultInteract,
    FlashlightEnable,
    FlashlightFocus,
    Healing,
    Movement,
    Sprint,
    Crouch,
    Tablet,
    Journal,
}

public static class InteractionTypeExtension
{
    private static InputActionAsset s_playerInputAssetValue;
    private const string PLAYER_INPUT_ACTION_LOCATION = "Input/PlayerInputActions";
    private static InputActionAsset s_playerInputAsset
    {
        get
        {
            // Null Check.
            s_playerInputAssetValue ??= Resources.Load<InputActionAsset>(PLAYER_INPUT_ACTION_LOCATION);

            // Return the value.
            return s_playerInputAssetValue;
        }
    }
    private static Dictionary<InteractionType, string> s_InteractionTypeToIdentifierDictionary = new Dictionary<InteractionType, string>()
    {
        {  InteractionType.DefaultInteract, "Interaction/Interact" },
        {  InteractionType.FlashlightEnable, "Interaction/ToggleFlashlight" },
        {  InteractionType.FlashlightFocus, "Interaction/FocusFlashlight" },
        {  InteractionType.Healing, "Interaction/UseHealingItem" },
        {  InteractionType.Movement, "Movement/Movement" },
        {  InteractionType.Sprint, "Movement/Sprint" },
        {  InteractionType.Crouch, "Movement/Crouch" },
        {  InteractionType.Tablet, "Global/PauseGame" },
        {  InteractionType.Journal, "Global/OpenJournal" },
    };


    public static Sprite GetInteractionSpriteFromInteractionType(InteractionType interactionType)
    {
        if (s_InteractionTypeToIdentifierDictionary.TryGetValue(interactionType, out string schemeName) == false)
        {
            Debug.LogError("Error: No Identifier set for Interaction Type: " + interactionType.ToString());
            throw new System.NotImplementedException();
        }

        Debug.Log(InputIconManager.GetIconForAction(s_playerInputAsset[schemeName]));
        return InputIconManager.GetIconForAction(s_playerInputAsset[schemeName]);
    }
    public static string GetInteractionSpriteIdentifierFromInteractionType(InteractionType interactionType)
    {
        if (s_InteractionTypeToIdentifierDictionary.TryGetValue(interactionType, out string schemeName) == false)
        {
            Debug.LogError("Error: No Identifier set for Interaction Type: " + interactionType.ToString());
            throw new System.NotImplementedException();
        }

        return InputIconManager.GetIconIdentifierForAction(s_playerInputAsset[schemeName]);
    }
}