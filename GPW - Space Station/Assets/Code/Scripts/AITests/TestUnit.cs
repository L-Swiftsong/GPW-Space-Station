using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding
{
    public class TestUnit : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _moveSpeed;

        private Vector3[] _path;
        private int _targetIndex;
        private Coroutine _followPathCoroutine;


        [Header("Debug")]
        [SerializeField] private bool _drawPathGizmos;
        [SerializeField] private float _pathWaypointSize = 0.2f;


        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);

            PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);
        }

        private void OnPathFound(Vector3[] newPath, bool wasSuccessful)
        {
            if (wasSuccessful)
            {
                _path = newPath;

                if (_followPathCoroutine != null)
                {
                    StopCoroutine(_followPathCoroutine);
                }
                Debug.Log(wasSuccessful);
                _followPathCoroutine = StartCoroutine(FollowPath());
            }
        }
        private IEnumerator FollowPath()
        {
            Vector3 currentWaypoint = _path[0];

            while(true)
            {
                if (transform.position == currentWaypoint)
                {
                    _targetIndex++;

                    if (_targetIndex >= _path.Length)
                    {
                        yield break;
                    }
                    currentWaypoint = _path[_targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, _moveSpeed * Time.deltaTime);
                yield return null;
            }
        }



        private void OnDrawGizmosSelected()
        {
            if (!_drawPathGizmos || _path == null)
            {
                return;
            }

            Gizmos.color = Color.black;
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.DrawCube(_path[i], Vector3.one * _pathWaypointSize);

                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i - 1], _path[i]);
                }
            }
        }
    }
}