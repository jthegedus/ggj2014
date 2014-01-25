using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using GGJ2014.GameObjects;
using GGJ2014.AI;
using Microsoft.Xna.Framework;
using GGJ2014.Levels;

namespace GGJ2014.Controllers
{
    class AIController : IController
    {
        private Agent agent;
        private Path path;

        public Agent Target { get; set; }

        private Vector2 oldGridPos = Vector2.Zero;

        public const float RecalculateTime = 0.5f;
        private float recalculateTimer = RecalculateTime;
        public AIController(Agent agent)
        {
            this.agent = agent;
        }

        public void HandleInput()
        {

            if (agent.Enabled)
            {
                Level level = TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level;
                Vector2 gridPos = new Vector2(level.GetGridX(agent.TransformComponent.Position.X), level.GetGridY(agent.TransformComponent.Position.Y));

                if (Target != null)
                {
                    Vector2 targetGrid = new Vector2(level.GetGridX(Target.TransformComponent.Position.X), level.GetGridY(Target.TransformComponent.Position.Y));
                    path = Path.pathfind(gridPos, targetGrid, level);
                    oldGridPos = Vector2.Zero;
                }
                if (path != null && (!gridPos.Equals(oldGridPos) || oldGridPos.Equals(Vector2.Zero)))
                {
                    oldGridPos = gridPos;

                    // Move towards waypoint
                    while (path.Waypoints.Count > 0 && Vector2.Distance(gridPos, path.Waypoints[0]) < 2.5f)
                    {
                        path.Waypoints.RemoveAt(0);
                    }
                    Vector2 move = Vector2.Zero;
                    if (path.Waypoints.Count > 0)
                    {
                        // Vector towards goal
                        move = (path.Waypoints[0] - gridPos) * 1f;
                        move.Y *= -1;
                        // Apply steering with nearest adjacent rectangles
                        Vector2[] adjacents = new Vector2[8] { new Vector2(gridPos.X-1, gridPos.Y-1),
                                                               new Vector2(gridPos.X, gridPos.Y-1),
                                                               new Vector2(gridPos.X+1, gridPos.Y-1),
                                                               new Vector2(gridPos.X+1, gridPos.Y),
                                                               new Vector2(gridPos.X+1, gridPos.Y+1),
                                                               new Vector2(gridPos.X, gridPos.Y+1),
                                                               new Vector2(gridPos.X-1, gridPos.Y+1),
                                                               new Vector2(gridPos.X-1, gridPos.Y) };
                        for (int i = 0; i < adjacents.Length; ++i)
                        {
                            // If adjacent cell is a wall
                            if (level.getCell((int)adjacents[i].X, (int)adjacents[i].Y) == false)
                            {
                                // Steer away from that bitch
                                Vector2 vec = gridPos - adjacents[i];
                                vec.Y *= -1;
                                move += vec;
                            }
                        }
                        if (!move.Equals(Vector2.Zero))
                            move.Normalize();
                    }
                    this.agent.DesiredMovementDirection = move; // Vector towards next waypoint
                }
            }
        }
    }
}
