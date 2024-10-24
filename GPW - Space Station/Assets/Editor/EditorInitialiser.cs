using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;


// Credit: 'https://discussions.unity.com/t/load-required-scenes-before-awake-start-of-objects-when-entering-play-mode-from-level-scene/849922/8'.
[InitializeOnLoad]
public class EditorInitialiser
{
    // Persistent Scene Path.
    private const string PERSISTENT_SCENE_PATH = "Assets/Level/Scenes/PersistentScene.unity";

    // Editor Preferences save names.
    private const string ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER = "PreviousScenePath";
    private const string EDITOR_INITIALISED_PREF_IDENTIFIER = "EditorInitialisation";


    // The names of the scenes which we wish to do the Editor Initialisation on.
    private static List<string> _invalidScenes = new List<string>
    {
        Path.GetFileNameWithoutExtension(PERSISTENT_SCENE_PATH),
    };
    // The names of the scenes which we wish to load in addition to the first scene. Loaded in the order they appear in the List.
    private static List<string> _extraScenesToLoad = new List<string>
    {
        
    };


    static EditorInitialiser()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    ~EditorInitialiser()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode && IsValidScene(_invalidScenes, out string sceneName))
        {
            EditorPrefs.SetString(ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER, sceneName);
            EditorPrefs.SetBool(EDITOR_INITIALISED_PREF_IDENTIFIER, true);
            SetStartScene(PERSISTENT_SCENE_PATH);
        }

        if (state == PlayModeStateChange.EnteredPlayMode && EditorPrefs.GetBool(EDITOR_INITIALISED_PREF_IDENTIFIER))
        {
            LoadExtraScenes();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            EditorPrefs.SetBool(EDITOR_INITIALISED_PREF_IDENTIFIER, false);
        }
    }

    private static void SetStartScene(string scenePath)
    {
        SceneAsset firstSceneToLoad = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

        if (firstSceneToLoad != null)
        {
            // We found the scene we wished to load. Load it.
            EditorSceneManager.playModeStartScene = firstSceneToLoad;
        }
        else
        {
            // We couldn't find the scene we wished to load.
            Debug.LogErrorFormat("Could not find the first scene to load (Path: {0})", scenePath);
        }
    }
    private static void LoadExtraScenes()
    {
        // Load extra scenes.
        foreach (string scenePath in _extraScenesToLoad)
        {
            SceneManager.LoadScene(scenePath, LoadSceneMode.Additive);
        }

        // Load the original scene.
        string originalScene = EditorPrefs.GetString(ACTIVE_EDITOR_SCENE_PREF_IDENTIFIER);
        SceneManager.LoadSceneAsync(originalScene, LoadSceneMode.Additive);

        // Figure out a way to have the previous scene be set to the active one (Check out the UniTask repo).
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(originalScene));
    }

    private static bool IsValidScene(List<string> invalidScenes, out string sceneName)
    {
        sceneName = SceneManager.GetActiveScene().name;
        return !invalidScenes.Contains(sceneName);
    }
}
