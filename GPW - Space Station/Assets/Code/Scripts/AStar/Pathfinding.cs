using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding.AStar
{
    [RequireComponent(typeof(Grid))]
    public class Pathfinding : MonoBehaviour
    {
        private Grid _grid = null;
        private Heap<Node> _openSet = null;
        private HashSet<Node> _closedSet = new HashSet<Node>();

        private const int GRID_DIAGONAL_DISTANCE = 14;
        private const int GRID_HORIZONTAL_DISTANCE = 10;


        [Header("Testing")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _seekerTransform;


        private void Awake()
        {
            _grid = GetComponent<Grid>();
        }

        private void Update()
        {
            FindPath(_seekerTransform.position, _playerTransform.position);
        }


        private void FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _grid.GetNodeFromWorldPoint(startPos);
            Node targetNode = _grid.GetNodeFromWorldPoint(targetPos);

            // Initialise the openSet if we haven't done so already.
            if (_openSet == null)
            {
                _openSet = new Heap<Node>(_grid.MaxSize);
            }
            
            // Clear the openSet and closedSet.
            _openSet.Clear();
            _closedSet.Clear();
            // Add the start node to the openSet.
            _openSet.Add(startNode);

            while(_openSet.Count > 0)
            {
                Node currentNode = _openSet.RemoveFirst();
                _closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    // We have found the target.
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.IsWalkable)
                    {
                        // The neightbour isn't walkable.
                        continue;
                    }
                    if (_closedSet.Contains(neighbour))
                    {
                        // The neightbour has already been evaluated.
                        continue;
                    }
                    

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.GCost || !_openSet.Contains(neighbour))
                    {
                        // Calculate the neighbour node's new gCost & hCost (Subsequently also the fCost).
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);

                        // Set the neighbour's parent for path retracing.
                        neighbour.Parent = currentNode;

                        if (!_openSet.Contains(neighbour))
                        {
                            // Add the neighbour to the open set.
                            _openSet.Add(neighbour);
                        }
                    }
                }
            }
        }


        private void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            // Work backwards from the end node to get our path.
            Node currentNode = endNode;
            while(currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            // Reverse the path to get the correct order.
            path.Reverse();

            _grid.CurrentPath = path;
        }

        public int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int yDistance = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (xDistance < yDistance)
            {
                // xDistance represents diagonal moves.
                return (xDistance * GRID_DIAGONAL_DISTANCE) + ((yDistance - xDistance) * GRID_HORIZONTAL_DISTANCE);
            }
            else
            {
                // yDistance represents diagonal moves.
                return (yDistance * GRID_DIAGONAL_DISTANCE) + ((xDistance - yDistance) * GRID_HORIZONTAL_DISTANCE);
            }
        }
    }
}