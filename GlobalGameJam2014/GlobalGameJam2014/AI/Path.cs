using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Levels;
using GGJ2014.Components;
using Microsoft.Xna.Framework;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using GGJ2014.Graphics;

namespace GGJ2014.AI
{
    public class Path : IDraw
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

            public override string ToString()
            {
                return "Node [" + Pos.X + ", " + Pos.Y + "] gCost: " + gCost + " fCost: " + fCost;
            }
        }

        public List<Vector2> Waypoints { get; private set; }

        public Path()
        {
            Waypoints = new List<Vector2>();
        }

        public static Path pathfind(Vector2 origin, Vector2 target, Level level)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();
            openList.Add(new Node(origin, calculateFCost(origin, target)));

            Node current = null;

            // Create adjacent vector array here, to avoid re-allocating each node
            Vector2[] adjacentVectors = { Vector2.Zero, Vector2.Zero, Vector2.Zero,
                                        Vector2.Zero, Vector2.Zero, Vector2.Zero, 
                                        Vector2.Zero, Vector2.Zero};

            while (openList.Count > 0)
            {
                // Get first node (lowest cost) and remove from open list (add to closed)
                current = openList[0];
                if(current.Pos.Equals(target))
                {
                    break;
                }
                openList.RemoveAt(0);
                closedList.Add(current);
                Node[] adjacentNodes = getAdjacents(adjacentVectors, current, level);
                foreach(Node adjacent in adjacentNodes)
                {
                    if (adjacent != null && !isInClosed(closedList, adjacent))
                    {
                        // Calculate fCost (need target to do so)
                        adjacent.fCost = calculateFCost(adjacent.Pos, target);
                        // Adds node to list if not found
                        // OR
                        // If node found with higher cost, replaces with new parent and cost
                        tryAddToOpen(adjacent, openList);
                    }
                }
            }
            
            if(current != null)
            {
                Path path = new Path();
                // Construct path and return
                while (current != null)
                {
                    path.Waypoints.Add(current.Pos);
                    current = current.Parent;
                }
                path.Waypoints.Reverse();
                return path;
            }
            return null;
        }

        private static Node[] getAdjacents(Vector2[] positions, Node node, Level level)
        {
            positions[0].X = node.Pos.X-1; positions[0].Y = node.Pos.Y-1;
            positions[1].X = node.Pos.X;   positions[1].Y = node.Pos.Y-1;
            positions[2].X = node.Pos.X+1; positions[2].Y = node.Pos.Y-1;
            positions[3].X = node.Pos.X+1; positions[3].Y = node.Pos.Y;
            positions[4].X = node.Pos.X+1; positions[4].Y = node.Pos.Y+1;
            positions[5].X = node.Pos.X;   positions[5].Y = node.Pos.Y+1;
            positions[6].X = node.Pos.X-1; positions[6].Y = node.Pos.Y+1;
            positions[7].X = node.Pos.X-1; positions[7].Y = node.Pos.Y;

            Node[] adjacents = new Node[positions.Length];
            for(int i = 0; i < positions.Length; ++i)
            {
                Vector2 pos = positions[i];
                // Check for level bounds and walkable
                if (pos.X >= 0 && pos.Y >= 0 && pos.X < level.Width && pos.Y < level.Height && level.getCell((int)pos.X, (int)pos.Y))
                {
                    float gCost = node.gCost + (float)Math.Sqrt(Math.Pow(node.Pos.X - pos.X, 2) + Math.Pow(node.Pos.Y - pos.Y, 2));
                    adjacents[i] = new Node(pos, node, gCost, 0);
                }
            }
            return adjacents;
        }

        private static bool isInClosed(List<Node> nodes, Node node)
        {
            foreach (Node n in nodes)
            {
                if (n.Pos.Equals(node.Pos))
                    return true;
            }
            return false;
        }

        private static int calculateFCost(Vector2 pos1, Vector2 pos2)
        {
            return (int)(Math.Abs(pos1.X - pos2.X) + Math.Abs(pos1.Y - pos2.Y));
        }

        private static void tryAddToOpen(Node newNode, List<Node> list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Node node = list[i];
                // If node exists in open list
                if (node.Pos.Equals(newNode.Pos))
                {
                    // If node in list has higher cost, replace
                    if(newNode.gCost < node.gCost)
                        list[i] = newNode;
                    return;
                }
                // Sorted list, so need to add just in front of next biggest fCost
                if (node.fCost > newNode.fCost)
                {
                    list.Insert(i, newNode);
                    return;
                }
            }
            list.Add(newNode);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Color color = Color.Green;
            color.A = 128;
            // Draw dem waypoints
            foreach (Vector2 waypoint in Waypoints)
            {
                Sprite sprite = TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level.sprite;
                sprite.Tint = color;
                Vector2 drawPos = new Vector2((sprite.Width/2) + waypoint.X * sprite.Width, (sprite.Height/2) + waypoint.Y * sprite.Height);
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level.Sprite.Draw(spriteBatch, drawPos);
            }
        }
    }
}
