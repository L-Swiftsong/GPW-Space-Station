using UnityEngine;

// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace Mimicry.PassiveMimicry
{
    public class PassiveMimicryController : MonoBehaviour
    {
        [SerializeField] private PassiveMimicryRenderer[] _passiveMimicryRenderers;

        [SerializeField] [Range(0.0f, 1.0f)] private float _passiveMimicryRamp = 0.0f;


        private void Update()
        {
            // Loop through each PassiveMimicryRenderer and update it's passiveMimicryRamp (Strength).
            for (int i = 0; i < _passiveMimicryRenderers.Length; i++)
            {
                _passiveMimicryRenderers[i].PassiveMimicryRamp = _passiveMimicryRamp;
            }
        }

        public void SetPassiveMimicryRamp(float newValue) => _passiveMimicryRamp = Mathf.Clamp01(newValue);
    }
}