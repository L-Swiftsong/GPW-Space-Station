using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// From: 'https://vfxmike.blogspot.com/2018/06/opaque-adaptive-camouflage.html'.
namespace GPW.Tests.Camouflage
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class ActiveCamoCommandBuffer : MonoBehaviour
    {
        public static ActiveCamoCommandBuffer Instance { get; private set; }


        private CommandBuffer _rbDrawAC;
        [SerializeField] private CameraEvent _rbDrawACQueue = CameraEvent.AfterForwardOpaque;

        private HashSet<ActiveCamoObject> _activeCamoObjects = new HashSet<ActiveCamoObject>();
        private Camera _thisCamera;
        private bool _updateActiveCamoCB = false;


        private void Awake()
        {
            ActiveCamoCommandBuffer.Instance = this;
        }
        private void OnEnable()
        {
            _thisCamera = GetComponent<Camera>();

            // Create the DrawActiveCamo CommandBuffer.
            _rbDrawAC = new CommandBuffer();
            _rbDrawAC.name = "DrawActiveCamo";
            _thisCamera.AddCommandBuffer(_rbDrawACQueue, _rbDrawAC);
            _updateActiveCamoCB = true;
        }
        private void OnDisable()
        {
            if (_rbDrawAC != null)
            {
                // Remove the DrawActiveCamo CommandBuffer from the camera.
                _thisCamera.RemoveCommandBuffer(_rbDrawACQueue, _rbDrawAC);
                _rbDrawAC = null;
            }
        }


        /// <summary> Add the passed ActiveCamoObject to the acObjects to be accounted for in the buffer.</summary>
        public void AddRenderer(ActiveCamoObject newObject)
        {
            _activeCamoObjects.Add(newObject);
            _updateActiveCamoCB = true;
        }
        /// <summary> Remove the passed ActiveCamoObject from the acObjects to be accounted for in the buffer.</summary><param name="newObject">
        public void RemoveRenderer(ActiveCamoObject newObject)
        {
            _activeCamoObjects.Remove(newObject);
            _updateActiveCamoCB = true;
        }


        // Rebuild the DrawActiveCammo command buffer.
        private void RebuildCBActiveCamo()
        {
            _rbDrawAC.Clear();

            foreach(ActiveCamoObject activeCamoObject in _activeCamoObjects)
            {
                _rbDrawAC.DrawRenderer(activeCamoObject.Renderer, activeCamoObject.Material);
            }

            _updateActiveCamoCB = false;
        }

        private void OnPreRender()
        {
            if (_updateActiveCamoCB)
            {
                // The command buffer needs rebuilt.
                RebuildCBActiveCamo();
            }
        }
    }
}