using UnityEngine;
using UnityEngine.Rendering;


// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace Mimicry.PassiveMimicry
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class FrameGrabCommandBuffer : MonoBehaviour
    {
        private CommandBuffer _rbFrame;
        [SerializeField] private CameraEvent _rbFrameQueue = CameraEvent.AfterForwardAlpha;

        public RenderTexture LastFrame;
        public RenderTexture LastFrameTemp;
        public RenderTargetIdentifier LastFrameRTI;

        private int _screenX = 0;
        private int _screenY = 0;
        private Camera _thisCamera;


        private const string PASSIVE_MIMICRY_GLOBAL_FLOAT_IDENTIFIER = "_GlobalPassiveMimicry";
        private const string LAST_FRAME_GLOBAL_TEXTURE_IDENTIFIER = "_LastFrame";


        private void OnEnable()
        {
            // Grab the camera component.
            _thisCamera = GetComponent<Camera>();

            // Create & setup the Command Buffer.
            _rbFrame = new CommandBuffer();
            _rbFrame.name = "Frame Capture";
            _thisCamera.AddCommandBuffer(_rbFrameQueue, _rbFrame);

            RebuildCBFrame();

            Shader.SetGlobalFloat(PASSIVE_MIMICRY_GLOBAL_FLOAT_IDENTIFIER, 1.0f);
        }
        private void OnDisable()
        {
            // Remove the command buffer from the camera.
            if (_rbFrame != null)
            {
                _thisCamera.RemoveCommandBuffer(_rbFrameQueue, _rbFrame);
                _rbFrame = null;
            }


            // Clean up the last frame texture.
            if (LastFrame != null)
            {
                LastFrame.Release();
                LastFrame = null;
            }


            // Ensure that no camo shaders attempt to render while there isn't a 'FrameGrabCommandBuffer' active.
            Shader.SetGlobalFloat(PASSIVE_MIMICRY_GLOBAL_FLOAT_IDENTIFIER, 0.0f);
        }


        private void RebuildCBFrame()
        {
            // Clear the buffer so that there are no instructions left within.
            _rbFrame.Clear();

            // If the last frame texture already exists, then the screen size will have changed.
            if (LastFrame != null)
            {
                // Store the existing lastFrame in the lastFrameTemp variable.
                LastFrameTemp = RenderTexture.GetTemporary(LastFrame.width, LastFrame.height, 0, RenderTextureFormat.DefaultHDR);
                Graphics.Blit(LastFrame, LastFrameTemp);

                // Release the lastFrame to dealocate it from memory.
                LastFrame.Release();
                LastFrame = null;
            }


            // Store the current width & height of the camera.
            _screenX = _thisCamera.pixelWidth;
            _screenY = _thisCamera.pixelHeight;


            // Create the lastFrame RenderTexture.
            // Make a new render texture for the last frame (Half the resolution is fine).
            LastFrame = new RenderTexture(_screenX / 2, _screenY / 2, 0, RenderTextureFormat.DefaultHDR);

            // Ensure that we clamp the renderTexture so that we don't accidentally pull from the other side of the screen when distored.
            LastFrame.wrapMode = TextureWrapMode.Clamp;

            // Create the render texture & get a RenderTargetIdentifier for it.
            LastFrame.Create();
            LastFrameRTI = new RenderTargetIdentifier(LastFrame);


            if (LastFrameTemp != null)
            {
                // We've got stored data in the lastFrameTemp texture.
                // Copy the data from the lastFrameTemp texture into the lastFrame texture.
                Graphics.Blit(LastFrameTemp, LastFrame);

                // Release the lastFrameTemp texture from memory.
                RenderTexture.ReleaseTemporary(LastFrameTemp);
                LastFrameTemp = null;
            }


            // Inform the shaders what Texture they should use for their camo.
            Shader.SetGlobalTexture(LAST_FRAME_GLOBAL_TEXTURE_IDENTIFIER, LastFrame);


            // Get the RTI of the camera and copy its pixel data to the lastFrameRTI.
            RenderTargetIdentifier cameraTargetID = new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget);
            _rbFrame.Blit(cameraTargetID, LastFrameRTI);
        }


        private void OnPreRender()
        {
            if (_screenX != _thisCamera.pixelWidth || _screenY != _thisCamera.pixelHeight)
            {
                // The screen size has changed.
                // Rebuild the CommandBuffer.
                RebuildCBFrame();
            }
        }
    }
}