using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI.Pathfinding.AStar
{
    public class Node : IHeapItem<Node>
    {
        public Vector3 WorldPosition;
        public int GridX, GridY;
        public bool IsWalkable;

        public Node Parent;


        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;

        public int HeapIndex { get; set; }


        public Node(Vector3 worldPosition, int gridX, int gridY, bool isWalkable)
        {
            this.WorldPosition = worldPosition;
            this.GridX = gridX;
            this.GridY = gridY;
            this.IsWalkable = isWalkable;

            this.Parent = null;

            this.GCost = 0;
            this.HCost = 0;
        }


        public int CompareTo(Node nodeToCompare)
        {
            int comparison = FCost.CompareTo(nodeToCompare.FCost);

            if (comparison == 0)
            {
                // FCosts are equal.
                // Use HCost as a tiebreaker.
                comparison = HCost.CompareTo(nodeToCompare.HCost);
            }

            // The CompareTo of Integer has 1 has higher. We want 1 to be lower.
            return -comparison;
        }
    }
}