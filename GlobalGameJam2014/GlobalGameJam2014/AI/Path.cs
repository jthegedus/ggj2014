using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Levels;
using GGJ2014.Components;
using Microsoft.Xna.Framework;

namespace GGJ2014.AI
{
    public class Path
    {
        private class Node
        {
            public Node Parent;
            // Holds Grid coordinates!! Not screen position!
            public Vector2 Pos;
            // Cost to move from origin to this node
            public float gCost;
            // Heuristic cost to move from here to target
            public float fCost;

            public Node(Vector2 pos, Node parent, float gCost, float fCost)
            {
                this.Pos = pos;
                this.Parent = parent;
                this.gCost = gCost;
            }

            public Node(Vector2 pos, float fCost)
                : this(pos, null, 0, fCost)
            {
            }
        }

        private List<Vector2> Waypoints { get; private set; }

        public static Path pathfind(Vector2 origin, Vector2 target, Level level)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            openList.Add(new Node(origin));

            while (openList.Count > 0)
            {
                // Get first node (lowest cost) and remove from open list (add to closed)
                Node current = openList[0];
                openList.RemoveAt(0);
                closedList.Add(current);
                Node[] adjacentNodes = getAdjacents(current, level);
                foreach(Node adjacent in adjacentNodes)
                {
                    if (adjacent != null)
                    {
                        // Calculate fCost (need target to do so)

                        // Adds node to list if not found
                        // OR
                        // If node found with higher cost, replaces with new parent and cost
                        tryAddToOpen(adjacent, openList);
                    }
                }
            }
            return null;
        }

        private static Node[] getAdjacents(Node node, Level level)
        {
            Vector2[] positions = { new Vector2(node.Pos.X-1, node.Pos.Y-1), 
                                  new Vector2(node.Pos.X, node.Pos.Y-1),
                                  new Vector2(node.Pos.X+1, node.Pos.Y-1), 
                                  new Vector2(node.Pos.X+1, node.Pos.Y),
                                  new Vector2(node.Pos.X+1, node.Pos.Y+1), 
                                  new Vector2(node.Pos.X, node.Pos.Y+1),
                                  new Vector2(node.Pos.X-1, node.Pos.Y+1),
                                  new Vector2(node.Pos.X-1, node.Pos.Y)};
            Node[] adjacents = new Node[positions.Length];
            for(int i = 0; i < positions.Length; ++i)
            {
                Vector2 pos = positions[i];
                // Check for level bounds and walkable
                if (pos.X >= 0 && pos.Y >= 0 && pos.X < level.Width && pos.Y < level.Height && level.getCell((int)pos.X, (int)pos.Y))
                {
                    float gCost = node.gCost + (float)Math.Sqrt(Math.Pow(node.Pos.X - pos.X, 2) + Math.Pow(node.Pos.Y - pos.Y, 2));
                    adjacents[i] = new Node(pos, node, gCost);
                }
            }
            return adjacents;
        }

        private static void tryAddToOpen(Node newNode, List<Node> list)
        {
            for(int i = 0; i < list.Count; ++i)
            {
                Node node = list[i];
                if(node.gCost > newNode.gCost
        }
    }
}
