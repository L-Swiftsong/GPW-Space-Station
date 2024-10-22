using UnityEngine;

// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace GPW.Tests.Camouflage
{
    public class ActiveCamoController : MonoBehaviour
    {
        [SerializeField] private ActiveCamoRenderer[] _activeCamoRenderers;

        [SerializeField] [Range(0.0f, 1.0f)] private float _activeCampRamp = 0.0f;


        private void Update()
        {
            for (int i = 0; i < _activeCamoRenderers.Length; i++)
            {
                _activeCamoRenderers[i].ActiveCamoRamp = _activeCampRamp;
            }
        }

        public void SetActiveCamoRamp(float newValue) => _activeCampRamp = Mathf.Clamp01(newValue);
    }
}