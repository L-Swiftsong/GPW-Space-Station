using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] public float delayBeforeFloat = 2f;
    [SerializeField] private float floatDistance = 3f;
    [SerializeField] private float floatDuration = 3f;

    private void Start()
    {
        StartCoroutine(FloatUp());
    }

    private IEnumerator FloatUp()
    {
        yield return new WaitForSeconds(delayBeforeFloat);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, floatDistance, 0);
        float timer = 0f;

        while (timer < floatDuration)
        {
            timer += Time.deltaTime;
            float t = timer / floatDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
    }
}
