using SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        SceneLoader.OnHardLoadStarted += SceneLoader_OnHardLoadStarted;
        SceneLoader.OnLoadFinished += Hide;
    }
    private void OnDisable()
    {
        SceneLoader.OnHardLoadStarted -= SceneLoader_OnHardLoadStarted;
        SceneLoader.OnLoadFinished -= Hide;
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
        _container.SetActive(true);
    }
    private void Hide()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _container.SetActive(false);
    }
}
