using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI.Pathfinding
{
    [RequireComponent(typeof(Pathfinding))]
    public class PathRequestManager : MonoBehaviour
    {
        private static PathRequestManager _instance;


        private Pathfinding _pathfindingScript;

        private Queue<PathRequest> _pathRequestQueue = new Queue<PathRequest>();
        private PathRequest _currentPathRequest;
        private bool _isProcessingRequest;


        private void Awake()
        {
            // Setup singleton instance.
            if (_instance != null)
            {
                Debug.LogError(string.Format("Error: A PathRequestManager instance already exists ({0}). Destroying {1}", _instance.name, this.name));
                Destroy(this.gameObject);
                return;
            }

            _instance = this;


            // Get Pathfinding reference.
            _pathfindingScript = GetComponent<Pathfinding>();
        }


        public static void RequestPath(Vector3 startPos, Vector3 targetPos, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(startPos, targetPos, callback);
            _instance._pathRequestQueue.Enqueue(newRequest);
            _instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!_isProcessingRequest && _pathRequestQueue.Count > 0)
            {
                _currentPathRequest = _pathRequestQueue.Dequeue();
                _isProcessingRequest = true;
                _pathfindingScript.StartFindPath(_currentPathRequest.PathStart, _currentPathRequest.PathEnd);
            }
        }


        public void FinishedProcessingPath(Vector3[] path, bool wasSuccessful)
        {
            _currentPathRequest.Callback(path, wasSuccessful);
            _isProcessingRequest = false;
            TryProcessNext();
        }


        struct PathRequest
        {
            public Vector3 PathStart;
            public Vector3 PathEnd;
            public Action<Vector3[], bool> Callback;


            public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
            {
                this.PathStart = pathStart;
                this.PathEnd = pathEnd;
                this.Callback = callback;
            }
        }
    }
}