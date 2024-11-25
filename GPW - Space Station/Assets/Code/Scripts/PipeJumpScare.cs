using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeJumpScare : MonoBehaviour
{
    public ParticleSystem SteamParticleSystem;
    public float steamDuration = 10f;
    public float fadeDuration = 2f;

    private bool isActive = false;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !isActive)
        {
            JumpScare();
        }
    }

    private void JumpScare()
    {
        if (SteamParticleSystem != null)
        {
            isActive = true;
            StartCoroutine(EnableSteam());
        }
    }

    IEnumerator EnableSteam()
    {
        audioSource.Play();

        yield return new WaitForSeconds(0.5f); // This is just to sync better with audio

        SteamParticleSystem.Play();

        yield return new WaitForSeconds(steamDuration);

        var mainModule = SteamParticleSystem.main;
        float startLifetime = mainModule.startLifetime.constant;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            mainModule.startLifetime = Mathf.Lerp(startLifetime, 0f, progress);

            yield return null;
        }

        SteamParticleSystem.Stop();

        audioSource.loop = false;
        audioSource.Stop();
    }
}
