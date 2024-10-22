using UnityEngine;

// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace GPW.Tests.Camouflage
{
    public class ActiveCamoRenderer : MonoBehaviour
    {
        private Renderer _thisRenderer;

        [SerializeField] private Material _activeCamoMaterial;
        private MaterialPropertyBlock _materialPropertyBlock;
        private ActiveCamoObject _activeCamoObject;
        [HideInInspector] public float ActiveCamoRamp = 0.0f;


        private const string ACTIVE_CAMO_RAMP_IDENTIFIER = "_ActiveCamoRamp";


        private void Start()
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            _thisRenderer = GetComponent<Renderer>();

            // Create and setup the ActiveCamoObject.
            _activeCamoObject = new ActiveCamoObject();
            _activeCamoObject.Renderer = _thisRenderer;
            _activeCamoObject.Material = _activeCamoMaterial;
        }

        
        private void OnBecameVisible() => ActiveCamoCommandBuffer.Instance.AddRenderer(_activeCamoObject);
        private void OnBecameInvisible() => ActiveCamoCommandBuffer.Instance.RemoveRenderer(_activeCamoObject);


        private void Update()
        {
            _materialPropertyBlock.SetFloat(ACTIVE_CAMO_RAMP_IDENTIFIER, ActiveCamoRamp);
            _thisRenderer.SetPropertyBlock(_materialPropertyBlock);
        }
    }
}