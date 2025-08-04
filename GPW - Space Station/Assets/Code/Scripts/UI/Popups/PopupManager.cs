using System;
using System.Collections;
using System.Collections.Generic;
using UI.Icons;
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
        [SerializeField] private ScreenSpacePopupElement _screenSpaceSingleLinePopupPrefab;
        private ObjectPool<ScreenSpacePopupElement> _screenSpaceSingleLinePopupPool;

        [SerializeField] private ScreenSpacePopupElement _screenSpaceMultiLinePopupPrefab;
        private ObjectPool<ScreenSpacePopupElement> _screenSpaceMultiLinePopupPool;


        [Header("World-Space Popups")]
        [SerializeField] private WorldSpacePopupElement _worldSpaceSingleLinePopupPrefab;
        private ObjectPool<WorldSpacePopupElement> _worldSpaceSingleLinePopupPool;

        [Space(5)]
        [SerializeField] private WorldSpacePopupElement _worldSpaceMultiLinePopupPrefab;
        private ObjectPool<WorldSpacePopupElement> _worldSpaceMultiLinePopupPool;


        [Header("Tutorial Popup Settings")]
        [SerializeField] private Vector2 _tutorialPopupPosition;
        [SerializeField] private Vector2 _tutorialPopupPivot;
        

        [Header("Interaction Settings")]
        [SerializeField] private InputActionAsset _playerInputAsset;

        private static Dictionary<InteractionType, string> s_interactionTypeToIdentifierDictionary = new Dictionary<InteractionType, string>()
        {
            {  InteractionType.DefaultInteract, "Interaction/Interact" },
            {  InteractionType.FlashlightEnable, "Interaction/ToggleFlashlight" },
            {  InteractionType.FlashlightFocus, "Interaction/FocusFlashlight" },
            {  InteractionType.Healing, "Interaction/UseHealingItem" },
            {  InteractionType.Movement, "Movement/Movement" },
            {  InteractionType.Sprint, "Movement/Sprint" },
            {  InteractionType.Crouch, "Movement/Crouch" },
        };



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
            _screenSpaceSingleLinePopupPool = new ObjectPool<ScreenSpacePopupElement>(createFunc: CreateScreenSpaceSingleLinePopup, actionOnGet: OnGetScreenSpacePopup, actionOnRelease: OnReleaseScreenSpacePopup);
            _screenSpaceMultiLinePopupPool = new ObjectPool<ScreenSpacePopupElement>(createFunc: CreateScreenSpaceMultiLinePopup, actionOnGet: OnGetScreenSpacePopup, actionOnRelease: OnReleaseScreenSpacePopup);

            _worldSpaceSingleLinePopupPool = new ObjectPool<WorldSpacePopupElement>(createFunc: CreateWorldSpaceSingleLinePopup, actionOnGet: OnGetWorldSpacePopup, actionOnRelease: OnReleaseWorldSpacePopup);
            _worldSpaceMultiLinePopupPool = new ObjectPool<WorldSpacePopupElement>(createFunc: CreateWorldSpaceMultiLinePopup, actionOnGet: OnGetWorldSpacePopup, actionOnRelease: OnReleaseWorldSpacePopup);
        }


        #region Object Pool

        #region Screen Space

        private ScreenSpacePopupElement CreateScreenSpaceSingleLinePopup()
        {
            ScreenSpacePopupElement element = Instantiate<ScreenSpacePopupElement>(_screenSpaceSingleLinePopupPrefab, _screenSpacePopupRoot);
            return element;
        }
        private ScreenSpacePopupElement CreateScreenSpaceMultiLinePopup()
        {
            ScreenSpacePopupElement element = Instantiate<ScreenSpacePopupElement>(_screenSpaceMultiLinePopupPrefab, _screenSpacePopupRoot);
            return element;
        }

        private void OnGetScreenSpacePopup(ScreenSpacePopupElement popupElement)
        {
            popupElement.gameObject.SetActive(true);
        }
        private void OnReleaseScreenSpacePopup(ScreenSpacePopupElement popupElement)
        {
            popupElement.gameObject.SetActive(false);
        }

        #endregion

        #region World Space

        private WorldSpacePopupElement CreateWorldSpaceSingleLinePopup()
        {
            WorldSpacePopupElement element = Instantiate<WorldSpacePopupElement>(_worldSpaceSingleLinePopupPrefab, this.transform);
            return element;
        }
        private WorldSpacePopupElement CreateWorldSpaceMultiLinePopup()
        {
            WorldSpacePopupElement element = Instantiate<WorldSpacePopupElement>(_worldSpaceMultiLinePopupPrefab, this.transform);
            return element;
        }

        private void OnGetWorldSpacePopup(WorldSpacePopupElement popupElement)
        {
            popupElement.gameObject.SetActive(true);
        }
        private void OnReleaseWorldSpacePopup(WorldSpacePopupElement popupElement)
        {
            popupElement.gameObject.SetActive(false);
        }

        #endregion

        #endregion


        public static void CreateScreenSpacePopup(ScreenSpacePopupSetupInformation setupInformation)
        {
            ObjectPool<ScreenSpacePopupElement> utilisedPool = setupInformation.DisplayOnMultipleLines ? s_instance._screenSpaceMultiLinePopupPool : s_instance._screenSpaceSingleLinePopupPool;
            ScreenSpacePopupElement popupElement = utilisedPool.Get();

            popupElement.SetupWithInformation(setupInformation, s_instance.GetInteractionSpriteFromInteractionType(setupInformation.InteractionType), () => utilisedPool.Release(popupElement));
        }


        [System.Serializable] public enum InteractionType { DefaultInteract, FlashlightEnable, FlashlightFocus, Healing, Movement, Sprint, Crouch }
        public static void CreateWorldSpacePopup(WorldSpacePopupSetupInformation popupSetupInformation)
        {
            ObjectPool<WorldSpacePopupElement> utilisedPool = popupSetupInformation.DisplayOnMultipleLines ? s_instance._worldSpaceMultiLinePopupPool : s_instance._worldSpaceSingleLinePopupPool;
            WorldSpacePopupElement popupElement = utilisedPool.Get();

            popupElement.SetupWithInformation(popupSetupInformation, s_instance.GetInteractionSpriteFromInteractionType(popupSetupInformation.InteractionType), () => utilisedPool.Release(popupElement));
        }



        private Sprite GetInteractionSpriteFromInteractionType(InteractionType interactionType)
        {
            if (s_interactionTypeToIdentifierDictionary.TryGetValue(interactionType, out string schemeName) == false)
            {
                Debug.LogError("Error: No Identifier set for Interaction Type: " + interactionType.ToString());
                throw new System.NotImplementedException();
            }

            Debug.Log(InputIconManager.GetIconForAction(_playerInputAsset[schemeName]));
            return InputIconManager.GetIconForAction(_playerInputAsset[schemeName]);
        }
        public static string GetInteractionSpriteIdentifierFromInteractionType_Static(InteractionType interactionType) => s_instance.GetInteractionSpriteIdentifierFromInteractionType(interactionType);
        private string GetInteractionSpriteIdentifierFromInteractionType(InteractionType interactionType)
        {
            if (s_interactionTypeToIdentifierDictionary.TryGetValue(interactionType, out string schemeName) == false)
            {
                Debug.LogError("Error: No Identifier set for Interaction Type: " + interactionType.ToString());
                throw new System.NotImplementedException();
            }

            return InputIconManager.GetIconIdentifierForAction(_playerInputAsset[schemeName]);
        }
    }
}