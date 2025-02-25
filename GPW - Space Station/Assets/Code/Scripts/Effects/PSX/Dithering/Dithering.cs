using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class Dithering : VolumeComponent, IPostProcessComponent
    {
        //PIXELATION
        //public TextureParameter ditherTexture;
        public ClampedIntParameter patternIndex = new ClampedIntParameter(0, 0, 3);
        public FloatParameter ditherThreshold = new FloatParameter(512);
        public FloatParameter ditherStrength = new FloatParameter(1);
        public ClampedFloatParameter ditherFadeStrength = new ClampedFloatParameter(1.0f, 0.0f, 1.0f);
        public FloatParameter ditherScale = new FloatParameter(2);
        
        
        //INTERFACE REQUIREMENT 
        public bool IsActive() => true;
        public bool IsTileCompatible() => false;
    }
}