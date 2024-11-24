using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Modals;
using UnityEngine.Pool;
using UI.Animations;
using Interaction;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LimitedDurationModalUI _tutorialModalPrefab;
    [SerializeField] private Transform _tutorialModalContainer;
    private ObjectPool<LimitedDurationModalUI> _tutorialModalPool;

    private struct TutorialContents
    {
        public string Text { get; }
        public Sprite Sprite { get; }

        public TutorialContents(string text, Sprite sprite)
        {
            this.Text = text;
            this.Sprite = sprite;
        }
    }
    private Queue<TutorialContents> _queuedTutorials = new Queue<TutorialContents>();
    private Coroutine _displayTutorialsCoroutine;


    [Header("Defaults")]
    [SerializeField] private Sprite _defaultTutorialSprite;
    [SerializeField] private float _tutorialModalLifetime = 2.5f;


    [Header("Triggered Tutorials")]
    private static bool s_hasTriggeredMovementTutorial = false;
    private static bool s_hasTriggeredCameraTutorial = false;
    private static bool s_hasTriggeredInteractionTutorial = false;


    private void Awake()
    {
        _tutorialModalPool = new ObjectPool<LimitedDurationModalUI>(createFunc: CreateTutorialModalInstance, actionOnGet: OnGetTutorialModalInstance, actionOnRelease: OnReleaseTutorialModalInstance);


        SetupTutorialMessages();
    }

    private void SetupTutorialMessages()
    {
        // Setup movement tutorial.
        if (!s_hasTriggeredMovementTutorial)
        {
            EnqueueTutorialMessage(new TutorialContents("Use [WASD] or [Left Stick] to move", null), includeInitialDelay: true);
            s_hasTriggeredMovementTutorial = true;
        }
        // Setup camera tutorial.
        if (!s_hasTriggeredCameraTutorial)
        {
            EnqueueTutorialMessage(new TutorialContents("Use [Mouse Movement] or [Right Stick] to look around", null), includeInitialDelay: true);
            s_hasTriggeredCameraTutorial = true;
        }

        // Setup interaction tutorial.
        if (!s_hasTriggeredInteractionTutorial)
        {
            PlayerInteraction.OnHighlightedInteractableObject += PlayerInteraction_OnHighlightedInteractableObject;
        }
    }
    private void PlayerInteraction_OnHighlightedInteractableObject()
    {
        // Display the tutorial message.
        EnqueueTutorialMessage(new TutorialContents("Press [LMB] or [Button West] to interact", null));

        // Cache the fact that we have triggered this tutorial.
        s_hasTriggeredInteractionTutorial = true;

        // Unsubscribe from the event.
        PlayerInteraction.OnHighlightedInteractableObject -= PlayerInteraction_OnHighlightedInteractableObject;
    }


    private void EnqueueTutorialMessage(TutorialContents contents, bool includeInitialDelay = false)
    {
        _queuedTutorials.Enqueue(contents);
        TryResolveTutorialMessage(includeInitialDelay);
    }
    private void TryResolveTutorialMessage(bool includeInitialDelay)
    {
        if (_displayTutorialsCoroutine != null)
        {
            return;
        }

        _displayTutorialsCoroutine = StartCoroutine(ResolveTutorialMessages(includeInitialDelay));
    }
    private IEnumerator ResolveTutorialMessages(bool includeInitialDelay = false)
    {
        WaitForSeconds minDelayBetweenMessages = new WaitForSeconds(2.0f);

        if (includeInitialDelay)
        {
            float initialDelay = 2.5f;
            yield return new WaitForSeconds(initialDelay);
        }

        while (_queuedTutorials.Count > 0)
        {
            // Display the next queued tutorial.
            TutorialContents tutorialContents = _queuedTutorials.Dequeue();
            DisplayTutorialMessage(tutorialContents.Text, tutorialContents.Sprite);

            // Wait until we have stopped displaying our tutorial modal.
            yield return new WaitUntil(() => _tutorialModalPool.CountActive == 0);

            // Wait the minimum delay between messages.
            yield return minDelayBetweenMessages;
        }

        // Ensure that we can run this coroutine again.
        _displayTutorialsCoroutine = null;
    }


    #region ObjectPool

    private LimitedDurationModalUI CreateTutorialModalInstance()
    {
        // Instantiate the modal.
        LimitedDurationModalUI modalInstance = Instantiate<LimitedDurationModalUI>(_tutorialModalPrefab, _tutorialModalContainer);

        // Setup the modal's animations.
        UIElementTranslation modalAnimation = modalInstance.GetComponent<UIElementTranslation>();
        modalInstance.OnModalEnabled += modalAnimation.StartAnimation;
        modalInstance.OnModalDurationElapsed += modalAnimation.StartReverseAnimation;
        modalInstance.OnModalDurationElapsed += () => StartCoroutine(ReleaseAfterDelay(modalInstance, modalAnimation.GetDuration()));

        modalInstance.gameObject.SetActive(false);
        return modalInstance;
    }
    private void OnGetTutorialModalInstance(LimitedDurationModalUI modal) => modal.gameObject.SetActive(true);
    private void OnReleaseTutorialModalInstance(LimitedDurationModalUI modal) => modal.gameObject.SetActive(false);

    #endregion



    public void DisplayTutorialMessage(string tutorialMessage, Sprite tutorialSprite = null)
    {
        LimitedDurationModalUI tutorialModal = _tutorialModalPool.Get();

        tutorialModal.SetModal(tutorialMessage, tutorialSprite != null ? tutorialSprite : _defaultTutorialSprite, _tutorialModalLifetime);
    }
    private IEnumerator ReleaseAfterDelay(LimitedDurationModalUI modal, float delay)
    {
        yield return new WaitForSeconds(delay);
        _tutorialModalPool.Release(modal);
    }
}
