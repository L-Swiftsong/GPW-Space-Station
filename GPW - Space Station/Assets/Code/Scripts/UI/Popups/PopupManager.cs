using System;
using System.Collections;
using System.Collections.Generic;
using UI.Icons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace UI.Popups
{
    public partial class PopupManager : MonoBehaviour
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

            popupElement.SetupWithInformation(setupInformation, InteractionTypeExtension.GetInteractionSpriteFromInteractionType(setupInformation.InteractionType), () => utilisedPool.Release(popupElement));
        }
        public static void CreateWorldSpacePopup(WorldSpacePopupSetupInformation popupSetupInformation)
        {
            ObjectPool<WorldSpacePopupElement> utilisedPool = popupSetupInformation.DisplayOnMultipleLines ? s_instance._worldSpaceMultiLinePopupPool : s_instance._worldSpaceSingleLinePopupPool;
            WorldSpacePopupElement popupElement = utilisedPool.Get();

            popupElement.SetupWithInformation(popupSetupInformation, InteractionTypeExtension.GetInteractionSpriteFromInteractionType(popupSetupInformation.InteractionType), () => utilisedPool.Release(popupElement));
        }        
    }
}