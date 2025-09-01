using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        /// <summary> A list of our current requests, with indices corresponding to the integer value of 'AnchorPosition'. </summary>
        // Note: We're not using a queue as we want to request to remain until after it is processed, not until we first start processing it.
        private List<ScreenSpacePopupRequest>[] _activeAnchorPositionCounts;


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
            // Setup our Popup Object Pools.
            _worldSpaceSingleLinePopupPool = new ObjectPool<WorldSpacePopupElement>(createFunc: CreateWorldSpaceSingleLinePopup, actionOnGet: OnGetWorldSpacePopup, actionOnRelease: OnReleaseWorldSpacePopup);
            _worldSpaceMultiLinePopupPool = new ObjectPool<WorldSpacePopupElement>(createFunc: CreateWorldSpaceMultiLinePopup, actionOnGet: OnGetWorldSpacePopup, actionOnRelease: OnReleaseWorldSpacePopup);

            _screenSpaceSingleLinePopupPool = new ObjectPool<ScreenSpacePopupElement>(createFunc: CreateScreenSpaceSingleLinePopup, actionOnGet: OnGetScreenSpacePopup, actionOnRelease: OnReleaseScreenSpacePopup);

            // Create and initialise the '_activeAnchorPositionCounts' array.
            _activeAnchorPositionCounts = new List<ScreenSpacePopupRequest>[(int)AnchorPosition.ValueCount];
            for (int i = 0; i < (int)AnchorPosition.ValueCount; ++i)
                _activeAnchorPositionCounts[i] = new List<ScreenSpacePopupRequest>();
        }


        #region Object Pool

        #region Screen Space

        private ScreenSpacePopupElement CreateScreenSpaceSingleLinePopup()
        {
            ScreenSpacePopupElement element = Instantiate<ScreenSpacePopupElement>(_screenSpaceSingleLinePopupPrefab, _screenSpacePopupRoot);
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


        public static void CreateWorldSpacePopup(WorldSpacePopupSetupInformation popupSetupInformation, PopupTextData textData, Transform pivotTransform, Vector3 popupPosition, bool rotateInPlace = true, GameObject linkedInteractable = null, bool linkToSuccess = true, bool linkToFailure = false)
        {
            ObjectPool<WorldSpacePopupElement> utilisedPool = s_instance._worldSpaceSingleLinePopupPool;
            WorldSpacePopupElement popupElement = utilisedPool.Get();

            popupElement.SetupWithInformation(popupSetupInformation, textData, pivotTransform, popupPosition, rotateInPlace, linkedInteractable, linkToSuccess, linkToFailure, () => utilisedPool.Release(popupElement));
        }


        /// <summary>
        ///     Creates a request for a Screen Space Popup which will appear once all prior requests for the AnchorPosition have concluded.
        /// </summary>
        public static void CreateScreenSpacePopup(ScreenSpacePopupSetupInformation setupInformation, GameObject linkedInteractable, bool linkToSuccess, bool linkToFailure, PopupTextData textData)
            => s_instance.EnqueueScreenSpacePopupRequest(setupInformation, linkedInteractable, linkToSuccess, linkToFailure, textData);
        /// <summary>
        ///     Creates a request for a Screen Space Popup which will appear once all prior requests for the AnchorPosition have concluded.
        /// </summary>
        public static void CreateScreenSpacePopup(ScreenSpacePopupSetupInformation setupInformation, PopupTextData textData)
            => CreateScreenSpacePopup(setupInformation, null, false, false, textData);
        /// <summary>
        ///     Creates and Enqueues a ScreenSpacePopupRequest, processing it if no other requests are being processed.
        /// </summary>
        private void EnqueueScreenSpacePopupRequest(ScreenSpacePopupSetupInformation setupInformation, GameObject linkedInteractable, bool linkToSuccess, bool linkToFailure, PopupTextData textData)
            => EnqueueScreenSpacePopupRequest(new ScreenSpacePopupRequest(setupInformation, linkedInteractable, linkToSuccess, linkToFailure, textData));
        /// <summary>
        ///     Enqueue a ScreenSpacePopupRequest, processing it if no other requests are being processed.
        /// </summary>
        private void EnqueueScreenSpacePopupRequest(ScreenSpacePopupRequest request)
        {
            int requestTypeIndex = (int)request.SetupInformation.AnchorPosition;    // Cached for readability.

            // Enqueue the request.
            _activeAnchorPositionCounts[requestTypeIndex].Add(request);

            // If we aren't currently processing any requests of this type (Count WAS 0, now 1), then process this request.
            if (_activeAnchorPositionCounts[requestTypeIndex].Count <= 1)
            {
                TryProcessNextRequest(requestTypeIndex);
            }
        }
        /// <summary>
        ///     If we have any requests in our queue, process them.
        /// </summary>
        private void TryProcessNextRequest(int requestTypeIndex)
        {
            if (_activeAnchorPositionCounts[requestTypeIndex].Count <= 0)
                return;

            // We have a request of this type. Process it.
            HandleNextScreenSpacePopupRequest(requestTypeIndex);
        }
        /// <summary>
        ///     Process the first ScreenSpacePopupRequest of the passed type index, removing it from the queue after it has finished displaying.
        /// </summary>
        private void HandleNextScreenSpacePopupRequest(int requestTypeIndex)
        {
            // Get our popup element from the corresponding pool.
            ObjectPool<ScreenSpacePopupElement> utilisedPool = s_instance._screenSpaceSingleLinePopupPool;
            ScreenSpacePopupElement popupElement = utilisedPool.Get();

            // Setup the popup element.
            ScreenSpacePopupRequest request = _activeAnchorPositionCounts[requestTypeIndex][0];
            popupElement.SetupWithInformation(request.SetupInformation, request.LinkedInteractable, request.LinkToSuccess, request.LinkToFailure, request.TextData, () => FinishRequestProcessing());


            // Function for cleaning up a popup once it has completed. Triggers the 'TryProcessNextRequest' call to keep processing requests.
            void FinishRequestProcessing()
            {
                // Release the popup object.
                utilisedPool.Release(popupElement);

                // Reduce the queued requests for this popup as the request has now been processed.
                _activeAnchorPositionCounts[requestTypeIndex].RemoveAt(0);

                // Try to Process the next Request.
                TryProcessNextRequest(requestTypeIndex);
            }
        }

        /// <summary>
        ///     A data container for a requested 'ScreenSpacePopup'.
        /// </summary>
        private struct ScreenSpacePopupRequest
        {
            public ScreenSpacePopupSetupInformation SetupInformation { get; }
            public GameObject LinkedInteractable { get; }
            public bool LinkToSuccess { get; }
            public bool LinkToFailure { get; }

            public PopupTextData TextData { get; }


            public ScreenSpacePopupRequest(ScreenSpacePopupSetupInformation setupInformation, GameObject linkedInteractable, bool linkToSuccess, bool linkToFailure, PopupTextData textData)
            {
                // Setup.
                this.SetupInformation = setupInformation;
                this.LinkedInteractable = linkedInteractable;
                this.LinkToSuccess = linkToSuccess;
                this.LinkToFailure = linkToFailure;

                // Text.
                this.TextData = textData;
            }
        }
    }
}