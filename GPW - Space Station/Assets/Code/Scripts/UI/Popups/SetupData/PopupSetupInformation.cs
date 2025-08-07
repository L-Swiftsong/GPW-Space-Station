using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Popups
{
    [System.Serializable]
    public abstract class PopupSetupInformation : ScriptableObject
    {
        [Header("Popup Information")]
        public float FontSize = 72.0f;


        [Header("Background")]
        public bool ShowBackground;


        [Header("General Deactivation Settings")]
        public float PopupLifetime = -1;


        [Header("Advanced Deactivation Settings")]
        public GameObject LinkedInteractable;

        [Space(5)]
        public bool LinkToSuccess = true;
        public bool LinkToFailure = true;


        private PopupSetupInformation() { }
        public PopupSetupInformation(float fontSize, float lifetime, bool showBackground)
        {
            this.FontSize = fontSize;

            this.PopupLifetime = lifetime;
            this.ShowBackground = showBackground;
        }
        public PopupSetupInformation(PopupSetupInformation other)
        {
            this.FontSize = other.FontSize;


            this.PopupLifetime = other.PopupLifetime;


            this.LinkedInteractable = other.LinkedInteractable;
            this.LinkToSuccess = other.LinkToSuccess;
            this.LinkToFailure = other.LinkToFailure;
        }
    }
}
