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


        [SerializeField] private Transform _passiveMimicryGFXRoot;
        private Renderer[] _passiveMimicryRenderers;
        private MaterialPropertyBlock _passiveMimicryMaterialPropertyBlock;


        [Header("Rendering Camera")]
        [SerializeField] private Camera _mimicryCamera;
        private Camera _playerCamera;
        private float _clipPlaneThreshold = 1.5f; // A default value of 1.5f seems to do the trick when we don't have a collider.


        private static readonly int PASSIVE_MIMICRY_STRENGTH_IDENTIFIER = Shader.PropertyToID("_PassiveMimicryStrength");


        private void Awake()
        {
            // Cache our passive mimicry renderers.
            _passiveMimicryRenderers = _passiveMimicryGFXRoot.GetComponentsInChildren<Renderer>();
            _passiveMimicryMaterialPropertyBlock = new MaterialPropertyBlock();
            for(int i = 0; i < _passiveMimicryRenderers.Length; ++i)
                _passiveMimicryRenderers[i].SetPropertyBlock(_passiveMimicryMaterialPropertyBlock);


            // Cache values for our mimicry render camera.
            _playerCamera = Camera.main;
            if (this.TryGetComponent<Collider>(out Collider collider))
            {
                _clipPlaneThreshold = collider.bounds.extents.magnitude;
            }
        }
        private void OnEnable() => UnityEngine.Rendering.RenderPipelineManager.beginContextRendering += RenderPipelineManager_beginContextRendering; // Use Camera.OnWillRenderObject to support multiple cameras?
        private void OnDisable() => UnityEngine.Rendering.RenderPipelineManager.beginContextRendering -= RenderPipelineManager_beginContextRendering; // Use Camera.OnWillRenderObject to support multiple cameras?


        private void RenderPipelineManager_beginContextRendering(UnityEngine.Rendering.ScriptableRenderContext context, List<Camera> cameras) => UpdateMimicryCamera();


        #region Renderer Updating

        public void SetMimicryStrengthTarget(float newStrength)
        {
            _targetMimicryStrength = Mathf.Clamp01(newStrength);

            if (_mimicryStrengthChangeCoroutine != null)
                StopCoroutine(_mimicryStrengthChangeCoroutine);
            _mimicryStrengthChangeCoroutine = StartCoroutine(MoveToTargetStrength());
        }

        public void InstantlySetMimicryStrength(float strength)
        {
            if (_mimicryStrengthChangeCoroutine != null)
                StopCoroutine(_mimicryStrengthChangeCoroutine);

            _currentMimicrystrength = Mathf.Clamp01(strength);
            _targetMimicryStrength = _currentMimicrystrength;
            UpdateMimicryRenderers();
        }

        private IEnumerator MoveToTargetStrength()
        {
            while(_currentMimicrystrength != _targetMimicryStrength)
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
            for (int i = 0; i < _passiveMimicryRenderers.Length; i++)
            {
                _passiveMimicryRenderers[i].GetPropertyBlock(_passiveMimicryMaterialPropertyBlock);
                _passiveMimicryMaterialPropertyBlock.SetFloat(PASSIVE_MIMICRY_STRENGTH_IDENTIFIER, _currentMimicrystrength);
                _passiveMimicryRenderers[i].SetPropertyBlock(_passiveMimicryMaterialPropertyBlock);
            }
        }

#endregion


        #region Camera Updating

        private void UpdateMimicryCamera()
        {
            if (_playerCamera == null || _mimicryCamera == null)
            {
                // We don't have a reference to one of the required cameras (Typically because we are reloading the scene).
                return;
            }

            _mimicryCamera.transform.position = _playerCamera.transform.position;
            _mimicryCamera.transform.rotation = _playerCamera.transform.rotation;
            SetMimicryCameraNearClipPlane(_playerCamera);
        }
        private void SetMimicryCameraNearClipPlane(Camera playerCamera)
        {
            // Calculate the desired position and forward of our clip plane, using our clipPlaneThreshold to ensure that we don't intsersect with our graphics.
            Vector3 clipPosition = Vector3.MoveTowards(transform.position, _playerCamera.transform.position, Mathf.Min(_clipPlaneThreshold, Vector3.Distance(transform.position, _mimicryCamera.transform.position)));
            Vector3 clipForward = Vector3.ProjectOnPlane((clipPosition - _mimicryCamera.transform.position).normalized, transform.up);
            int sign = System.Math.Sign(Vector3.Dot(clipForward, clipPosition - _mimicryCamera.transform.position));


            // Calculate the position of the clip plane in the space of our camera.
            Vector3 camSpacePos = _mimicryCamera.worldToCameraMatrix.MultiplyPoint(clipPosition);
            Vector3 camSpaceNormal = _mimicryCamera.worldToCameraMatrix.MultiplyVector(clipForward) * sign;
            float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal);

            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);


            // Update our projection matrix based on the new clip plane.
            // Note: Calculated with the player camera so that the player camera's settings (FoV, etc) are used.
            _mimicryCamera.projectionMatrix = playerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }

        #endregion
    }
}