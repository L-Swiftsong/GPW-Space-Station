using AI.Pathfinding.AStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNodesInView : MonoBehaviour
{
    [SerializeField] private Transform _orientation;


    [Space(5)]
    [SerializeField] private AI.Pathfinding.AStar.Grid _grid;

    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;


    private void Update()
    {
        _grid.GetNodesInViewCone(_orientation.position, _orientation.forward, _viewRadius, _viewAngle);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_orientation.position, _viewRadius);
    }
}
