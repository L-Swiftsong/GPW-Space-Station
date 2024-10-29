using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace Mimicry.PassiveMimicry
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class PassiveMimicryCommandBuffer : MonoBehaviour
    {
        public static PassiveMimicryCommandBuffer Instance { get; private set; }


        private CommandBuffer _rbDrawPassiveMimicry;
        [SerializeField] private CameraEvent _rbDrawPassiveMimicryQueue = CameraEvent.AfterForwardOpaque;

        private HashSet<PassiveMimicryObject> _passiveMimicryObjects = new HashSet<PassiveMimicryObject>();
        private Camera _thisCamera;
        private bool _updatePassiveMimicryCB = false;


        private void Awake()
        {
            PassiveMimicryCommandBuffer.Instance = this;
        }
        private void OnEnable()
        {
            _thisCamera = GetComponent<Camera>();

            // Create the DrawPassiveMimicry CommandBuffer.
            _rbDrawPassiveMimicry = new CommandBuffer();
            _rbDrawPassiveMimicry.name = "DrawPassiveMimicry";
            _thisCamera.AddCommandBuffer(_rbDrawPassiveMimicryQueue, _rbDrawPassiveMimicry);

            RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
            
            _updatePassiveMimicryCB = true;
        }
        private void OnDisable()
        {
            if (_rbDrawPassiveMimicry != null)
            {
                // Remove the DrawPassiveMimicry CommandBuffer from the camera.
                _thisCamera.RemoveCommandBuffer(_rbDrawPassiveMimicryQueue, _rbDrawPassiveMimicry);
                _rbDrawPassiveMimicry = null;
            }
        }


        /// <summary> Add the passed PassiveMimicryObject to the passiveMimicryObjects to be accounted for in the buffer.</summary>
        public void AddRenderer(PassiveMimicryObject newObject)
        {
            _passiveMimicryObjects.Add(newObject);
            _updatePassiveMimicryCB = true;
        }
        /// <summary> Remove the passed PassiveMimicryObject from the passiveMimicryObjects to be accounted for in the buffer.</summary>
        public void RemoveRenderer(PassiveMimicryObject newObject)
        {
            _passiveMimicryObjects.Remove(newObject);
            _updatePassiveMimicryCB = true;
        }


        // Rebuild the Passive Mimicry command buffer.
        private void RebuildPassiveMimicryCB()
        {
            // Clear the current command buffer.
            _rbDrawPassiveMimicry.Clear();

            // Update the CommandBuffer to draw to each PassiveMimicryObject's renderer.
            foreach(PassiveMimicryObject passiveMimicryObject in _passiveMimicryObjects)
            {
                _rbDrawPassiveMimicry.DrawRenderer(passiveMimicryObject.Renderer, passiveMimicryObject.Material);
            }

            _updatePassiveMimicryCB = false;
        }

        private void OnPreRender()
        {
            if (_updatePassiveMimicryCB)
            {
                // The command buffer needs rebuilt.
                RebuildPassiveMimicryCB();
            }
        }

        private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            OnPreRender();

            context.ExecuteCommandBuffer(_rbDrawPassiveMimicry);
            context.Submit();
        }
    }
}