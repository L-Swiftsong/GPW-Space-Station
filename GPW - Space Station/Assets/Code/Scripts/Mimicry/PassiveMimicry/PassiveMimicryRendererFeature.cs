using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PassiveMimicryRendererFeature : ScriptableRendererFeature
{
    [SerializeField] private PassiveMimicrySettings _settings;
    [SerializeField] private Shader _shader;
    private Material _material;
    private PassiveMimicryRenderPass _passiveMimicryRenderPass;
    
    // Called on the following events:
    //  - The Renderer Feature loads for the first time.
    //  - You enable or disable the Renderer Feature.
    //  - You change a property in the Inspector of the Renderer Feature.
    public override void Create()
    {
        if (_shader == null)
        {
            return;
        }

        _material = new Material(_shader);
        _passiveMimicryRenderPass = new PassiveMimicryRenderPass(_material, _settings);

        //_passiveMimicryRenderPass.renderPassEvent = RenderPassEvent.AfterForwardAlpha;
    }

    // This method is called every frame, once for each camera.
    // This method lets us inject ScriptableRenderPass instances into the scriptable renderer.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

    }
}


[System.Serializable]
public class PassiveMimicrySettings
{
    [Range(0.0f, 1.0f)] public float PassiveMimicryRamp;
}