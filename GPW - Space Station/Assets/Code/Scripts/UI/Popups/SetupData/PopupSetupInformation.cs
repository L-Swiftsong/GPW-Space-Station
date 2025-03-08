using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UI.Popups.PopupManager;

namespace UI.Popups
{
    [System.Serializable]
    public abstract class PopupSetupInformation
    {
        [Header("Popup Information")]
        public string PopupPreText = "Press";
        public InteractionType InteractionType = InteractionType.DefaultInteract;
        public string PopupPostText = "to Interact";

        [Space(5)]
        public float FontSize = 72.0f;
        public float IconSize = 100.0f;

        [Space(5)]
        public bool DisplayOnMultipleLines = false;
        public bool KeepIconCentred = false;


        [Header("Custom Text")]
        public bool UseCustomText = false;
        public string CustomPreText;
        public Sprite CustomSprite;
        public string CustomPostText;


        [Header("General Deactivation Settings")]
        public float PopupLifetime = -1;


        [Header("Advanced Deactivation Settings")]
        public GameObject LinkedInteractable;

        [Space(5)]
        public bool LinkToSuccess = true;
        public bool LinkToFailure = true;


        private PopupSetupInformation() { }
        public PopupSetupInformation(float fontSize, float iconSize, float lifetime)
        {
            this.FontSize = fontSize;
            this.IconSize = iconSize;

            this.PopupLifetime = lifetime;
        }
        public PopupSetupInformation(PopupSetupInformation other)
        {
            this.PopupPreText = other.PopupPreText;
            this.InteractionType = other.InteractionType;
            this.PopupPostText = other.PopupPostText;

            this.FontSize = other.FontSize;
            this.IconSize = other.IconSize;

            this.DisplayOnMultipleLines = other.DisplayOnMultipleLines;
            this.KeepIconCentred = other.KeepIconCentred;


            this.UseCustomText = other.UseCustomText;
            this.CustomPreText = other.CustomPreText;
            this.CustomSprite = other.CustomSprite;
            this.CustomPostText = other.CustomPostText;


            this.PopupLifetime = other.PopupLifetime;


            this.LinkedInteractable = other.LinkedInteractable;
            this.LinkToSuccess = other.LinkToSuccess;
            this.LinkToFailure = other.LinkToFailure;
        }
    }
}
