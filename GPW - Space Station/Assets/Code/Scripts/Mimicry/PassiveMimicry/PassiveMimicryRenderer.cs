using UnityEngine;

// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace Mimicry.PassiveMimicry
{
    public class PassiveMimicryRenderer : MonoBehaviour
    {
        private Renderer _thisRenderer;

        [SerializeField] private Material _passiveMimicryMaterial;
        private MaterialPropertyBlock _materialPropertyBlock;
        private PassiveMimicryObject _passiveMimicryObject;
        [HideInInspector] public float PassiveMimicryRamp = 0.0f;


        private const string PASSIVE_MIMICRY_RAMP_IDENTIFIER = "_PassiveMimicryRamp";


        private void Start()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            _thisRenderer = GetComponent<Renderer>();

            // Create and setup the PassiveMimicryObject.
            _passiveMimicryObject = new PassiveMimicryObject();
            _passiveMimicryObject.Renderer = _thisRenderer;
            _passiveMimicryObject.Material = _passiveMimicryMaterial;
        }

        
        private void OnBecameVisible() => PassiveMimicryCommandBuffer.Instance.AddRenderer(_passiveMimicryObject);
        private void OnBecameInvisible() => PassiveMimicryCommandBuffer.Instance.RemoveRenderer(_passiveMimicryObject);


        private void Update()
        {
            _materialPropertyBlock.SetFloat(PASSIVE_MIMICRY_RAMP_IDENTIFIER, PassiveMimicryRamp);
            _thisRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}