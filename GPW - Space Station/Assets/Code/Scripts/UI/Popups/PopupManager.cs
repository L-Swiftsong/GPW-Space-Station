using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace UI.Popups
{
    public class PopupManager : MonoBehaviour
    {
        private static PopupManager s_instance;


        [Header("Screen Popups")]
        [SerializeField] private RectTransform _screenSpacePopupRoot;

        [Space(5)]
        [SerializeField] private PopupElement _screenSpacePopupPrefab;
        private ObjectPool<PopupElement> _screenSpacePopupPool;


        [Header("World-Space Popups")]
        [SerializeField] private PopupElement _worldSpaceSingleLinePopupPrefab;
        private ObjectPool<PopupElement> _worldSpaceSingleLinePopupPool;

        [Space(5)]
        [SerializeField] private PopupElement _worldSpaceMultiLinePopupPrefab;
        private ObjectPool<PopupElement> _worldSpaceMultiLinePopupPool;
        

        [Header("Interaction Settings")]
        [SerializeField] private InputActionAsset _playerInputActions;


        [Header("Tutorial Popup Settings")]
        [SerializeField] private Vector2 _tutorialPopupPosition;
        [SerializeField] private Vector2 _tutorialPopupAnchors;


        private void Awake()
        {
            if (s_instance != null)
            {
                Debug.LogError($"Error: An instance of PopupManager already exists ({s_instance.name}). Destroying {this.name}");
                Destroy(this.gameObject);
                return;
            }
            s_instance = this;


            SetupPopupManager();
        }
        private void SetupPopupManager()
        {
            _screenSpacePopupPool = new ObjectPool<PopupElement>(createFunc: CreateScreenSpacePopup, actionOnGet: OnGetScreenSpacePopup, actionOnRelease: OnReleaseScreenSpacePopup);

            _worldSpaceSingleLinePopupPool = new ObjectPool<PopupElement>(createFunc: CreateWorldSpaceSingleLinePopup, actionOnGet: OnGetWorldSpacePopup, actionOnRelease: OnReleaseWorldSpacePopup);
            _worldSpaceMultiLinePopupPool = new ObjectPool<PopupElement>(createFunc: CreateWorldSpaceMultiLinePopup, actionOnGet: OnGetWorldSpacePopup, actionOnRelease: OnReleaseWorldSpacePopup);
        }


        #region Object Pool

        #region Screen Space

        private PopupElement CreateScreenSpacePopup()
        {
            PopupElement element = Instantiate<PopupElement>(_screenSpacePopupPrefab, _screenSpacePopupRoot);
            return element;
        }
        private void OnGetScreenSpacePopup(PopupElement popupElement)
        {
            popupElement.gameObject.SetActive(true);
        }
        private void OnReleaseScreenSpacePopup(PopupElement popupElement)
        {
            popupElement.gameObject.SetActive(false);
        }

        #endregion

        #region World Space

        private PopupElement CreateWorldSpaceSingleLinePopup()
        {
            PopupElement element = Instantiate<PopupElement>(_worldSpaceSingleLinePopupPrefab, this.transform);
            return element;
        }
        private PopupElement CreateWorldSpaceMultiLinePopup()
        {
            PopupElement element = Instantiate<PopupElement>(_worldSpaceMultiLinePopupPrefab, this.transform);
            return element;
        }

        private void OnGetWorldSpacePopup(PopupElement popupElement)
        {
            popupElement.gameObject.SetActive(true);
        }
        private void OnReleaseWorldSpacePopup(PopupElement popupElement)
        {
            popupElement.gameObject.SetActive(false);
        }

        #endregion

        #endregion


        public static void CreateScreenPopup(Vector2 popupPosition, Vector2 popupAnchors, string popupText)
        {

        }

        public static void CreateWorldSpacePopup(Transform pivotTransform, Vector3 offset, string popupText, bool alwaysDisplayOnTop = false, bool rotateInPlace = true)
        {

        }


        public static void CreateTutorialPopup(string popupText) => CreateScreenPopup(s_instance._tutorialPopupPosition, s_instance._tutorialPopupAnchors, popupText);

        [System.Serializable] public enum InteractionType { DefaultInteract, FlashlightEnable, FlashlightFocus, Healing, Crouch, Movement }
        public static void CreateInteractionPopup(Transform pivotTransform, Vector3 offset, bool rotateInPlace = true, string interactionPreText = "Press", InteractionType interactionType = InteractionType.DefaultInteract, string interactionPostText = "to Interact", bool interactionInformationOnNewLine = false)
        {
            ObjectPool<PopupElement> utilisedPool = interactionInformationOnNewLine ? s_instance._worldSpaceMultiLinePopupPool : s_instance._worldSpaceSingleLinePopupPool;
            PopupElement popupElement = utilisedPool.Get();

            popupElement.SetPosition(pivotTransform, offset, rotateInPlace);

            popupElement.SetInformation(interactionPreText, GetInteractionSpriteFromInteractionType(interactionType), interactionPostText);
        }


        private static Sprite GetInteractionSpriteFromInteractionType(InteractionType interactionType)
        {
            switch (interactionType)
            {
                case InteractionType.DefaultInteract:

                    return;
            }
        }
    }
}
