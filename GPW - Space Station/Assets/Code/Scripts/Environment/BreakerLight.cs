using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class BreakerLight : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Color _enabledColour;
        [SerializeField] private Color _disabledColour;


        public void SetIsEnabled(bool isEnabled) => _renderer.material.color = isEnabled ? _enabledColour : _disabledColour;
    }
}
