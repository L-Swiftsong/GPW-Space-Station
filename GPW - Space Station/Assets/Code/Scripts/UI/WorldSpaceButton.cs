using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Interaction;
using TMPro;

namespace UI.World
{
    public class WorldSpaceButton : MonoBehaviour, IInteractable
    {
        #region Interaction Parameters

        [field: SerializeField] public bool IsInteractable { get; set; } = true;

        public event System.Action OnSuccessfulInteraction;
        public event System.Action OnFailedInteraction;

        #endregion


        [Header("Text (Optional)")]
        [Tooltip("An optional reference for this button's text.")]
            [SerializeField] private TMP_Text _buttonText;


        [Header("Highlighting")]
        [SerializeField] private Graphic _targetGraphic;

        [Space(5)]
        [SerializeField] private bool _useColour = false;
        [SerializeField] private Color _defaultColour = new Color(1.0f, 1.0f, 1.0f);
        [SerializeField] private Color _highlightedColour = new Color(0.8f, 0.8f, 0.8f);

        [Space(5)]
        [SerializeField] private bool _useSpriteSwap = false;
        [SerializeField] private Sprite _defaultSprite;
        [SerializeField] private Sprite _highlightedSprite;


        private void Awake() => StopHighlighting();
        public void Interact(PlayerInteraction interactingScript)
        {
            OnSuccessfulInteraction?.Invoke();
        }
        public void Highlight()
        {
            if (_useColour)
                _targetGraphic.color = _highlightedColour;
            if (_useSpriteSwap)
                (_targetGraphic as Image).sprite = _highlightedSprite;
        }
        public void StopHighlighting()
        {
            if (_useColour)
                _targetGraphic.color = _defaultColour;
            if (_useSpriteSwap)
                (_targetGraphic as Image).sprite = _defaultSprite;
        }


        public void TrySetText(string displayText)
        {
            if (_buttonText != null)
                _buttonText.text = displayText;
        }
        public string TryGetText()
        {
            if (_buttonText != null)
            {
                return _buttonText.text;
            }

            return string.Empty;
        }


#if UNITY_EDITOR

        private void OnValidate()
        {
            if (_useSpriteSwap && (_targetGraphic == null || _targetGraphic is not Image))
            {
                Debug.LogError("Target Graphic needs to be an image if you are wanting to use Sprite Swap");
            }
        }

        #endif
    }
}
