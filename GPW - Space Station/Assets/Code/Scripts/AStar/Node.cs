using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI.Pathfinding.AStar
{
    public class Node
    {
        public Vector3 WorldPosition;
        public int GridX, GridY;
        public bool IsWalkable;

        public Node Parent;


        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;


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
    }
}