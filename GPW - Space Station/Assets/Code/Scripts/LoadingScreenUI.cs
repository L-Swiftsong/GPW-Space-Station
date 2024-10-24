using SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject _container;


    [Header("Loading Bar")]
    [SerializeField] private TMPro.TMP_Text _loadingText;



    private void Awake() => Hide();
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
            progress = SceneLoader.Instance.GetSceneLoadProgress();
            _loadingText.text = progress.ToString();
            yield return null;
        }

        float fullyLoadedDisplayDelay = 0.1f;
        yield return new WaitForSeconds(fullyLoadedDisplayDelay);

        // Finished loading.
        _loadingText.text = "Scene Loaded";
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
