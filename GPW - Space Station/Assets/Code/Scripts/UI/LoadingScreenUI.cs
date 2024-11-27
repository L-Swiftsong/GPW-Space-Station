using SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.GameOver;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject _container;


    [Header("Loading Progress")]
    [SerializeField] private GameObject _loadingProgressContainer;
    private CanvasGroup _loadingProgressCanvasGroup;

    [Space(5)]
    [SerializeField] private TMP_Text _currentProgressText;
    [SerializeField] private ProgressBar _loadingProgressBar;


    [Header("Loading Complete")]
    [SerializeField] private GameObject _loadingCompleteContainer;
    private CanvasGroup _loadingCompleteCanvasGroup;

    [Space(5)]
    [SerializeField] private float _fadeDelay = 0.2f;
    [SerializeField] private float _fadeDuration = 0.25f;



#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_loadingProgressContainer != null && _loadingProgressContainer.GetComponent<CanvasGroup>() == null)
        {
            Debug.LogError($"{this.name}'s LoadingScreenUI Instance's Loading Progress Container doesn't contain a Canvas Group instance.");
        }
        if (_loadingCompleteContainer != null && _loadingCompleteContainer.GetComponent<CanvasGroup>() == null)
        {
            Debug.LogError($"{this.name}'s LoadingScreenUI Instance's Loading Complete Container doesn't contain a Canvas Group instance.");
        }
    }
#endif


    private void Awake()
    {
        _loadingProgressCanvasGroup = _loadingProgressContainer.GetComponent<CanvasGroup>();
        _loadingCompleteCanvasGroup = _loadingCompleteContainer.GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        _loadingProgressBar.SetValues(current: 0.0f, max: 100.0f);
        _container.SetActive(false);
    }

    private void OnEnable()
    {
        // Subscribe to events.
        SceneLoader.OnHardLoadStarted += SceneLoader_OnHardLoadStarted;
        SceneLoader.OnLoadFinished += Hide;
        SceneLoader.OnMainMenuReloadFinished += HideWithoutCursorLock;
    }
    private void OnDisable()
    {
        // Unsubscribe from events.
        SceneLoader.OnHardLoadStarted -= SceneLoader_OnHardLoadStarted;
        SceneLoader.OnLoadFinished -= Hide;
        SceneLoader.OnMainMenuReloadFinished -= HideWithoutCursorLock;
    }



    private void SceneLoader_OnHardLoadStarted()
    {
        Show();
        StartCoroutine(DisplayLoadingProgress());
    }
    private IEnumerator DisplayLoadingProgress()
    {
        float progress = 0f;
        while(progress < 1.0f)
        {
            // Get the current progress percentage.
            progress = SceneLoader.Instance.GetSceneLoadProgress() * 100.0f;

            // Update the UI to show the current progress.
            _currentProgressText.text = Mathf.CeilToInt(progress).ToString() + "%";
            _loadingProgressBar.SetCurrentValue(progress);

            // Wait a frame between checks.
            yield return null;
        }

        // Finished loading.
        _currentProgressText.text = "100%";
        _loadingProgressBar.SetCurrentValue(100.0f);

        yield return new WaitForSecondsRealtime(_fadeDelay);

        // Slowly reveal the 'Loading Complete' visuals.
        StartCoroutine(SwapToLoadingCompleteContainer());
    }
    private IEnumerator SwapToLoadingCompleteContainer()
    {
        // Enable the LoadingCompleteContainer, but keep it hidden by starting its alpha at 0.
        _loadingCompleteCanvasGroup.alpha = 0.0f;
        _loadingCompleteContainer.SetActive(true);
        
        // Slowly reveal the LoadingCompleteContainer as we hide the LoadingProgressContainer.
        float lerpTime = 0.0f;
        while(lerpTime < 1.0f)
        {
            float alphaProgress = Mathf.Lerp(0.0f, 1.0f, lerpTime);
            _loadingCompleteCanvasGroup.alpha = alphaProgress;
            _loadingProgressCanvasGroup.alpha = 1.0f - alphaProgress;

            yield return new WaitForEndOfFrame();
            lerpTime += Time.unscaledDeltaTime / _fadeDuration;
        }

        _loadingCompleteCanvasGroup.alpha = 1.0f;

        // Disable the LoadingProgressContainer so that we don't keep calculating stuff for it.
        _loadingProgressContainer.SetActive(false);
    }



    private void Show()
    {
        Cursor.lockState = CursorLockMode.None;

        if (_container.activeSelf)
        {
            // We are already active. Don't proceed.
            return;
        }

        // Show ourself.
        _container.SetActive(true);

        // Start the loading progress as shown and the loading complete as hidden.
        _loadingProgressCanvasGroup.alpha = 1.0f;
        _loadingProgressContainer.SetActive(true);
        _loadingCompleteContainer.SetActive(false);

        // Disable Player Input for all non-UI actions.
        PlayerInput.PreventAllActions(typeof(LoadingScreenUI));
    }
    private void Hide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        HideWithoutCursorLock();
    }
    private void HideWithoutCursorLock()
    {
        _container.SetActive(false);
        PlayerInput.RemoveAllActionPrevention(typeof(LoadingScreenUI));
    }
}
