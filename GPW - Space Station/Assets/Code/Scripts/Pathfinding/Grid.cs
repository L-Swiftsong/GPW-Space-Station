using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI.Pathfinding.AStar
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private LayerMask _unwalkableMask;
        [SerializeField] private float _nodeRadius;
        private float _nodeDiameter;

        [SerializeField] private Vector2 _gridWorldSize;
        private int _gridSizeX, _gridSizeY;

        private Node[,] _grid;
        public int MaxSize => _gridSizeX * _gridSizeY;


        [Header("Gizmos")]
        [SerializeField] private bool _drawGridSizeGizmos;
        [SerializeField] private bool _drawNodeGizmos;


        private void Awake()
        {
            // Cache the diameter of each node.
            _nodeDiameter = _nodeRadius * 2.0f;

            // Determine the number of nodes on each axis and cache those values.
            _gridSizeX = Mathf.RoundToInt(_gridWorldSize.x / _nodeDiameter);
            _gridSizeY = Mathf.RoundToInt(_gridWorldSize.y / _nodeDiameter);

            // Create the grid.
            CreateGrid();
        }


        private void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeY];

            Vector3 worldBottomLeft = transform.position - (Vector3.right * (_gridWorldSize.x / 2.0f) + Vector3.forward * (_gridWorldSize.y / 2.0f));

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    // Determine the world position of this node.
                    Vector3 worldPosition = worldBottomLeft + new Vector3((x * _nodeDiameter) + _nodeRadius, 0.0f, (y * _nodeDiameter) + _nodeRadius);
                    
                    // Determine if this node is walkable.
                    bool isWalkable = !Physics.CheckSphere(worldPosition, _nodeRadius, _unwalkableMask);

                    _grid[x,y] = new Node(worldPosition, x, y, isWalkable);
                }
            }
        }


        public Node GetNodeFromWorldPoint(Vector3 worldPosition)
        {
            // Determnine the percentage distance of this point on the grid.
            float percentX = (worldPosition.x + _gridWorldSize.x / 2.0f) / _gridWorldSize.x;
            float percentY = (worldPosition.z + _gridWorldSize.y / 2.0f) / _gridWorldSize.y;

            // Clamp the percentages (In case the target is outwith the grid).
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);

            // Get the indicies of the node at this point.
            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);

            // Return the determined node.
            return _grid[x,y];
        }
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        // Don't include the testing node as one of its neighbours.
                        continue;
                    }
                    
                    int testX = node.GridX + x;
                    int testY = node.GridY + y;
                    if ((testX >= 0 && testX < _gridSizeX) && (testY >= 0 && testY < _gridSizeY))
                    {
                        neighbours.Add(_grid[testX, testY]);
                    }
                }
            }

            return neighbours;
        }


        private HashSet<Node> _nodesInView = new HashSet<Node>();
        public void GetNodesInViewCone(Vector3 coneOrigin, Vector3 coneForward, float coneLength, float coneAngle)
        {
            // Ensure that our cone origin has the same Y level as our grid (0.0f). (Prevents errors in angle calc).
            coneOrigin.y = 0.0f;

            // Clear our nodesInView.
            foreach (Node node in _nodesInView)
            {
                node.IsVisible = false;
            }
            _nodesInView.Clear();


            // Step 1: Test all nodes in a spherical radius.
            Node originNode = GetNodeFromWorldPoint(coneOrigin);
            float nodeTestDiameter = coneLength / _nodeRadius;
            float nodeTestRadius = nodeTestDiameter / 2.0f;

            int minX = Mathf.Max(0, Mathf.CeilToInt(originNode.GridX - nodeTestRadius));
            int maxX = Mathf.Min(_gridSizeX - 1, Mathf.FloorToInt(originNode.GridX + nodeTestRadius));
            int minY = Mathf.Max(0, Mathf.CeilToInt(originNode.GridY - nodeTestRadius));
            int maxY = Mathf.Min(_gridSizeY - 1, Mathf.FloorToInt(originNode.GridY + nodeTestRadius));

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!IsInsideCircle(originNode.GridX, originNode.GridY, x, y, nodeTestRadius))
                    {
                        // The node is outwith the circle.
                        continue;
                    }
                    if (Vector3.Angle(coneForward, _grid[x,y].WorldPosition - coneOrigin) > coneAngle / 2.0f)
                    {
                        // The node is not within the view angle.
                        continue;
                    }
                    

                    // The node is within the view cone.
                    _nodesInView.Add(_grid[x,y]);
                    _grid[x,y].IsVisible = true;
                }
            }
        }
        private bool IsInsideCircle(int centreX, int centreY, int targetX, int targetY, float radius)
        {
            float xDistance = centreX - targetX;
            float yDistance = centreY - targetY;

            float sqrDistance = xDistance * xDistance + yDistance * yDistance;
            return sqrDistance <= (radius * radius);
        }



        private void OnDrawGizmos()
        {
            if (_drawGridSizeGizmos)
            {
                // Draw the total size of the grid.
                Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));
            }


            // Draw each node in the grid.
            if (_grid != null && _drawNodeGizmos)
            {
                foreach (Node node in _grid)
                {
                    if (node.IsVisible)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = node.IsWalkable ? Color.white : Color.red;
                    }
                    
                    Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter - 0.1f));
                }
            }
        }
    }
}