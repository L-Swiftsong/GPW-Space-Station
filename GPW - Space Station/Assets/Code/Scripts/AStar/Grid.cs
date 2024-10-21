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
        [SerializeField] private bool _drawGizmos;
        public List<Node> CurrentPath { get; set; }


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


        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
            {
                return;
            }
            
            // Draw the total size of the grid.
            Gizmos.DrawWireCube(transform.position, new Vector3(_gridWorldSize.x, 1, _gridWorldSize.y));

            // Draw each node in the grid.
            if (_grid != null)
            {
                foreach (Node node in _grid)
                {
                    if (CurrentPath != null && CurrentPath.Contains(node))
                    {
                        Gizmos.color = Color.black;
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