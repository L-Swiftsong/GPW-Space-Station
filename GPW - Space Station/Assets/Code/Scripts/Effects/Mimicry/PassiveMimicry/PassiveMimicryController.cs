using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects.Mimicry.PassiveMimicry
{
    public class PassiveMimicryController : MonoBehaviour
    {
        [SerializeField] private float _strengthChangeRate;
        private float _currentMimicrystrength = 1.0f;
        private float _targetMimicryStrength = 1.0f;
        private Coroutine _mimicryStrengthChangeCoroutine;


        [SerializeField] private Renderer[] _defaultRenderers;
        [SerializeField] private Renderer[] _passiveMimicryRenderers;

        private MaterialPropertyBlock _defaultMaterialPropertyBlock;
        private MaterialPropertyBlock _passiveMimicryMaterialPropertyBlock;


        private static readonly int PASSIVE_MIMICRY_STRENGTH_IDENTIFIER = Shader.PropertyToID("_PassiveMimicryStrength");
        private static readonly int DEFAULT_MATERIAL_ALPHA_IDENTIFIER = Shader.PropertyToID("_Alpha");


        private void Awake()
        {
            // Cache our passive mimicry renderers.
            _passiveMimicryMaterialPropertyBlock = new MaterialPropertyBlock();
            for(int i = 0; i < _passiveMimicryRenderers.Length; ++i)
            {
                _passiveMimicryRenderers[i].SetPropertyBlock(_passiveMimicryMaterialPropertyBlock);
            }

            
            // Cache our default body & neck renderers.
            _defaultMaterialPropertyBlock = new MaterialPropertyBlock();
            for (int i = 0; i < _defaultRenderers.Length; ++i)
            {
                _defaultRenderers[i].SetPropertyBlock(_defaultMaterialPropertyBlock);
            }


            // Ensure that our renderers are correctly initialised.
            UpdateMimicryRenderers();
        }


        #region Renderer Updating

        public void SetMimicryStrengthTarget(float newStrength)
        {
            _targetMimicryStrength = Mathf.Clamp01(newStrength);

            if (_mimicryStrengthChangeCoroutine != null)
            {
                StopCoroutine(_mimicryStrengthChangeCoroutine);
            }
            _mimicryStrengthChangeCoroutine = StartCoroutine(MoveToTargetStrength());
        }

        public void InstantlySetMimicryStrength(float strength)
        {
            if (_mimicryStrengthChangeCoroutine != null)
            {
                StopCoroutine(_mimicryStrengthChangeCoroutine);
            }

            _currentMimicrystrength = Mathf.Clamp01(strength);
            _targetMimicryStrength = _currentMimicrystrength;
            UpdateMimicryRenderers();
        }

        private IEnumerator MoveToTargetStrength()
        {
            // Ensure that our renderers are up-to-date.
            UpdateMimicryRenderers();

            // Linearly transition towards our target strength.
            while (_currentMimicrystrength != _targetMimicryStrength)
            {
                // Update the mimicry renderers.
                _currentMimicrystrength = Mathf.MoveTowards(_currentMimicrystrength, _targetMimicryStrength, _strengthChangeRate * Time.deltaTime);
                UpdateMimicryRenderers();

                // Perform once per frame.
                yield return null;
            }
        }
        private void UpdateMimicryRenderers()
        {
            // Update the passive renderers.
            for (int i = 0; i < _passiveMimicryRenderers.Length; i++)
            {
                _passiveMimicryRenderers[i].GetPropertyBlock(_passiveMimicryMaterialPropertyBlock);
                _passiveMimicryMaterialPropertyBlock.SetFloat(PASSIVE_MIMICRY_STRENGTH_IDENTIFIER, _currentMimicrystrength);
                _passiveMimicryRenderers[i].SetPropertyBlock(_passiveMimicryMaterialPropertyBlock);
            }

            // Update the default renderers.
            for(int i = 0; i < _defaultRenderers.Length; ++i)
            {
                _defaultRenderers[i].GetPropertyBlock(_defaultMaterialPropertyBlock);
                _defaultMaterialPropertyBlock.SetFloat(DEFAULT_MATERIAL_ALPHA_IDENTIFIER, 1.0f - _currentMimicrystrength);
                _defaultRenderers[i].SetPropertyBlock(_defaultMaterialPropertyBlock);
            }
        }

#endregion
    }
}