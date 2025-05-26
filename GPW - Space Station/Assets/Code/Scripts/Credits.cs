using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Credits : MonoBehaviour
{
    [SerializeField] private TMP_Text _thanksForPlayingText;


    [Header("Credit Crawl Settings")]
    [SerializeField] private float _initialDelay = 5.0f;
    [SerializeField] private float _crawlSpeed = 4.5f;

    [Space(5)]
    [SerializeField] private Vector3 _startPosition = new Vector3(0.0f, 3.0f, 40.0f);
    [SerializeField] private Vector3 _endPosition = new Vector3(0.0f, 143.0f, 40.0f);


    [Header("Fade Settings")]
    [SerializeField] private float _fadeDelay = 2f;
    [SerializeField] private float _fadeDuration = 0.5f;


    private void Start() => StartCoroutine(FloatUp());
    

    private IEnumerator FloatUp()
    {
        // Ensure we are at our starting position.
        transform.position = _startPosition;


        yield return new WaitForSeconds(_initialDelay);


        // Perform the credits crawl.
        float timer = 0f;
        float creditCrawlDuration = Vector3.Distance(_startPosition, _endPosition) / _crawlSpeed;
        while (timer < creditCrawlDuration)
        {
            transform.position = Vector3.Lerp(_startPosition, _endPosition, timer / creditCrawlDuration);

            yield return null;
            timer += Time.deltaTime;
        }
        transform.position = _endPosition;


        // Fade the text out.
        yield return new WaitForSeconds(_fadeDelay);

        timer = 0f;
        while (timer < _fadeDuration)
        {
            _thanksForPlayingText.alpha = Mathf.Lerp(1.0f, 0.0f, timer / _fadeDuration);

            yield return null;
            timer += Time.deltaTime;
        }
        _thanksForPlayingText.alpha = 0.0f;
    }
}
