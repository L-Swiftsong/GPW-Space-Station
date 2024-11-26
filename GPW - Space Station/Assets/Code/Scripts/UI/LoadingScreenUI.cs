using SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.GameOver;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject _container;


    [Header("Loading Bar")]
    [SerializeField] private TMP_Text _currentProgressText;
    [SerializeField] private ProgressBar _loadingProgressBar;



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
    }



    private void Show()
    {
        Cursor.lockState = CursorLockMode.Confined;

        if (_container.activeSelf)
        {
            // We are already active. Don't proceed.
            return;
        }

        _container.SetActive(true);
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
