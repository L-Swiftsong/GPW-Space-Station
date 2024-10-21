using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    [RequireComponent(typeof(PathRequestManager))]
    public abstract class Pathfinding : MonoBehaviour
    {
        protected PathRequestManager PathRequestManager;

        protected virtual void Awake() => PathRequestManager = GetComponent<PathRequestManager>();
        


        public void StartFindPath(Vector3 startPosition, Vector3 targetPosition)
        {
            StartCoroutine(FindPath(startPosition, targetPosition));
        }

        protected abstract IEnumerator FindPath(Vector3 startPos, Vector3 targetPos);
    }
}