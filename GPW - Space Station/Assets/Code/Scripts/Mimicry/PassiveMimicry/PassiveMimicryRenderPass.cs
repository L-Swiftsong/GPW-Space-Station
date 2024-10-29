using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PassiveMimicryRenderPass : ScriptableRenderPass
{
    private PassiveMimicrySettings _defaultSettings;
    private Material _material;

    private RenderTextureDescriptor _passiveMimicryTextureDescriptor;


    // Variables for interacting with shader properties.
    private static readonly int _passiveMimicryRampID = Shader.PropertyToID("_PassiveMimicryRamp");


    public PassiveMimicryRenderPass(Material material, PassiveMimicrySettings defaultSettings)
    {
        this._material = material;
        this._defaultSettings = defaultSettings;

        this._passiveMimicryTextureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {

    }
    
    /*// This method adds and configures psases in the render graph. The process includes declaring inputs & outputs, but does not include adding commands to command buffers.
    // Called every frame, once for each camera.
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        // Get the UniversalResourceData of the current frame. This includes all texture references used by URP, including the active colour and depth textures of the camera.
        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        
    }*/
}
