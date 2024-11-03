using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveMimicryController : MonoBehaviour
{
    [SerializeField] private float _strengthChangeRate;
    private float _currentMimicrystrength = 1.0f;
    private float _targetMimicryStrength = 1.0f;
    private Coroutine _mimicryStrengthChangeCoroutine;


    [SerializeField] private Transform _passiveMimicryGFXRoot;
    private Renderer[] _passiveMimicryRenderers;


    private const string PASSIVE_MIMICRY_STRENGTH_IDENTIFIER = "_PassiveMimicryStrength";


    private void Awake() => _passiveMimicryRenderers = _passiveMimicryGFXRoot.GetComponentsInChildren<Renderer>();
    


    public void SetMimicryStrengthTarget(float newStrength)
    {
        _targetMimicryStrength = Mathf.Clamp01(newStrength);

        if (_mimicryStrengthChangeCoroutine != null)
            StopCoroutine(_mimicryStrengthChangeCoroutine);
        _mimicryStrengthChangeCoroutine = StartCoroutine(MoveToTargetStrength());
    }

    private IEnumerator MoveToTargetStrength()
    {
        while(_currentMimicrystrength != _targetMimicryStrength)
        {
            _currentMimicrystrength = Mathf.MoveTowards(_currentMimicrystrength, _targetMimicryStrength, _strengthChangeRate * Time.deltaTime);
            UpdateMimicryRenderers();
            yield return null;
        }
    }
    private void UpdateMimicryRenderers()
    {
        for (int i = 0; i < _passiveMimicryRenderers.Length; i++)
        {
            _passiveMimicryRenderers[i].material.SetFloat(PASSIVE_MIMICRY_STRENGTH_IDENTIFIER, _currentMimicrystrength);
        }
    }
}
