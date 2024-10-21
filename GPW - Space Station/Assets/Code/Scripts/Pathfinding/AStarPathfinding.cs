using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Pathfinding.AStar
{
    [RequireComponent(typeof(Grid))]
    public class AStarPathfinding : Pathfinding
    {
        private Grid _grid = null;
        private Heap<Node> _openSet = null;
        private HashSet<Node> _closedSet = new HashSet<Node>();

        private const int GRID_DIAGONAL_DISTANCE = 14;
        private const int GRID_HORIZONTAL_DISTANCE = 10;


        protected override void Awake()
        {
            base.Awake();
            _grid = GetComponent<Grid>();
        }

        protected override IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
        {
            Node startNode = _grid.GetNodeFromWorldPoint(startPos);
            Node targetNode = _grid.GetNodeFromWorldPoint(targetPos);

            if (!startNode.IsWalkable || !targetNode.IsWalkable)
            {
                PathRequestManager.FinishedProcessingPath(new Vector3[0], false);
                yield break;
            }


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
                    PathRequestManager.FinishedProcessingPath(RetracePath(startNode, targetNode), true);
                    break;
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

            yield return null;
            PathRequestManager.FinishedProcessingPath(new Vector3[0], false);
        }


        private Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();

            // Work backwards from the end node to get our path.
            Node currentNode = endNode;
            while(currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            // Reverse the waypoints to get the correct order.
            Vector3[] waypoints = SimplifyPath(path);
            System.Array.Reverse(waypoints);

            // Return the reversed list as an array of Vector3 points.
            return waypoints;
        }
        private Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i-1].GridX - path[i].GridX, path[i - 1].GridY - path[i].GridY);

                if (directionNew != directionOld)
                {
                    // The direction has changed.
                    waypoints.Add(path[i].WorldPosition);
                }

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        private int GetDistance(Node nodeA, Node nodeB)
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