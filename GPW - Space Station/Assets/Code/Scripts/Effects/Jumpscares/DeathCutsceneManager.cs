using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Audio;
using Entities.Player;
using UI.EndStates;
using SceneManagement;

public class DeathCutsceneManager : ProtectedSingleton<DeathCutsceneManager>
{
    [Header("Default Jumpscare Parameters")]
    [SerializeField] private float  _defaultJumpscareDuration = 0.15f;


    [Header("Default Audio Parameters")]
    [SerializeField] private AudioClipContainer _defaultDeathSFX;
    [SerializeField] private float _defaultDeathSFXDelay = 0.1f;

    [Space(5)]
    [SerializeField] private AudioClipContainer _defaultBiteSFX;
    [SerializeField] private float _defaultBiteSFXDelay = 1.0f;


    public static void TriggerDeathCutscene(Transform mimic, Transform mimicLookAt) => Instance.PerformDealthCutscene(mimic, mimicLookAt, OnDefaultCutsceneComplete);
    public static void TriggerDeathCutscene(Transform mimic, Transform mimicLookAt, System.Action onCompleteCallback, CameraFocusLook focusLookOverride) => Instance.PerformDealthCutscene(mimic, mimicLookAt, onCompleteCallback, focusLookOverride);
    private void PerformDealthCutscene(Transform mimic, Transform mimicLookAt, System.Action onCompleteCallback, CameraFocusLook focusLookOverride = null)
    {
        DeathCutsceneJumpscareInformation cutsceneInformation = new DeathCutsceneJumpscareInformation(_defaultJumpscareDuration);
        DeathCutsceneAudioInformation cutsceneAudioInformation = new DeathCutsceneAudioInformation(_defaultDeathSFX, _defaultDeathSFXDelay, _defaultBiteSFX, _defaultBiteSFXDelay);

        // Handle the Death Cutscene.
        StartCoroutine(DeathCutscene(mimic, mimicLookAt, cutsceneInformation, cutsceneAudioInformation, onCompleteCallback, focusLookOverride));
    }

    private IEnumerator DeathCutscene(Transform mimic, Transform mimicLookAt,
        DeathCutsceneJumpscareInformation cutsceneInformation, DeathCutsceneAudioInformation cutsceneAudioInformation,
        System.Action onCompleteCallback,
        CameraFocusLook focusLookOverride = null)
    {
        // Force the player to look at the mimic.
        if (focusLookOverride == null)
        {
            CameraFocusLook.TriggerFocusLookStatic(mimicLookAt.gameObject, 3f, 7.5f, PlayerInput.ActionTypes.Everything);
        }
        else
        {
            focusLookOverride.TriggerFocusLook(mimicLookAt.gameObject, 3f, 7.5f, PlayerInput.ActionTypes.Everything);
        }
        Transform playerCameraTransform = focusLookOverride != null ? focusLookOverride.transform : PlayerManager.Instance.GetPlayerCameraTransform();


        // Handle Audio.
        yield return StartCoroutine(HandleDeathAudio(mimicLookAt, cutsceneAudioInformation));

        // Wait to sync audio.
        yield return new WaitForSeconds(0.5f);

        // Handle the Jumpscare/Mimic Movement.
        yield return StartCoroutine(HandleDeathJumpscare(mimic, playerCameraTransform, cutsceneInformation));


        // Stop any Background Music overrides.
        BackgroundMusicManager.RemoveBackgroundMusicOverride();

        // Trigger the 'onCompleteCallback'.
        onCompleteCallback?.Invoke();
    }


    private IEnumerator HandleDeathAudio(Transform audioSourceTransform, DeathCutsceneAudioInformation cutsceneAudioInformation)
    {
        // Play the Intiial Audio.
        yield return new WaitForSeconds(cutsceneAudioInformation.DeathAudioClipDelay);
        AudioSource.PlayClipAtPoint(cutsceneAudioInformation.DeathAudioClipContainer.GetRandomClip(), audioSourceTransform.position, cutsceneAudioInformation.DeathAudioClipContainer.VolumeMultiplier);

        // Play the Bite Audio.
        yield return new WaitForSeconds(cutsceneAudioInformation.BiteAudioClipDelay);
        AudioSource.PlayClipAtPoint(cutsceneAudioInformation.BiteAudioClipContainer.GetRandomClip(), audioSourceTransform.position, cutsceneAudioInformation.BiteAudioClipContainer.VolumeMultiplier);
    }
    private IEnumerator HandleDeathJumpscare(Transform mimic, Transform cameraTransform, DeathCutsceneJumpscareInformation cutsceneInformation)
    {
        // Disable all mimic colliders.
        foreach (Collider collider in mimic.GetComponents<Collider>())
        {
            collider.enabled = false;
        }

        // Force the mimic to face the camera.
        Vector3 mimicToCameraDirection = cameraTransform.position - mimic.position;
        mimicToCameraDirection.y = 0.0f; // Zero the vertcal changes of direction to prevent the mimic leaning.
        mimic.rotation = Quaternion.LookRotation(mimicToCameraDirection.normalized); // Rotate the mimic to face the camera.


        // Calculate our required positions for movment.
        Vector3 startPosition = mimic.position;
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * -0.1f;
        targetPosition.y = transform.position.y;

        // Move the mimic towards the camera, keeping it grounded.
        float elapsedTime = 0.0f;
        while (elapsedTime <= cutsceneInformation.DeathCutsceneDuration)
        {
            Vector3 pos = Vector3.Lerp(startPosition, targetPosition, elapsedTime / cutsceneInformation.DeathCutsceneDuration);
            pos.y = startPosition.y;
            mimic.position = pos;

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // Ensure that movement was properly calculated.
        mimic.position = targetPosition;
    }


    private static void OnDefaultCutsceneComplete()
    {
        PlayerManager.Instance.Player.GetComponent<PlayerHealth>().DisableHealthVisors();
        GameOverUI.Instance.ShowGameOverUI();
    }
    



    #region Cutscene Information Classes

    private class DeathCutsceneJumpscareInformation
    {
        public float DeathCutsceneDuration;


        public DeathCutsceneJumpscareInformation(float deathCutsceneDuration)
        {
            DeathCutsceneDuration = deathCutsceneDuration;
        }
    }
    private class DeathCutsceneAudioInformation
    {
        public AudioClipContainer DeathAudioClipContainer;
        public float DeathAudioClipDelay;

        public AudioClipContainer BiteAudioClipContainer;
        public float BiteAudioClipDelay;


        public DeathCutsceneAudioInformation(AudioClipContainer deathAudioClipContainer, float deathAudioClipDelay, AudioClipContainer biteAudioClipContainer, float biteAudioClipDelay)
        {
            DeathAudioClipContainer = deathAudioClipContainer;
            DeathAudioClipDelay = deathAudioClipDelay;

            BiteAudioClipContainer = biteAudioClipContainer;
            BiteAudioClipDelay = biteAudioClipDelay;
        }
    }

    #endregion
}
