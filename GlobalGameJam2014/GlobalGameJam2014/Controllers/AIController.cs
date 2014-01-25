using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using GGJ2014.GameObjects;
using GGJ2014.AI;
using Microsoft.Xna.Framework;
using GGJ2014.Levels;
using GGJ2014.Components;

namespace GGJ2014.Controllers
{
    class AIController : IController, IUpdate
    {
        private Agent agent;
        private Path path;

        public ITransform Target { get; set; }

        private Vector2 oldGridPos = Vector2.Zero;

        public const float PathWeighting = 2f;
        public const float AvoidancePower = 4;
        public const float AvoidanceWeighting = 0.9f;
        public const float WaypointMargin = 1f;

        public  const int MinDecisionCooldown = 5;
        public const int MaxDecisionCooldown = 15;
        public const float MaxAttackDistance = 200f;
        public const float MinAttackDistance = 40f;
        private float decisionTimer = MinDecisionCooldown;

        public const float PathfindCooldown = 0.5f;
        private float pathfindTimer = PathfindCooldown;

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
                    // Uncomment these lines to draw path for AI!
                    //TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(path);
                    Vector2 targetGrid = new Vector2(level.GetGridX(Target.TransformComponent.Position.X), level.GetGridY(Target.TransformComponent.Position.Y));
                    path = Path.pathfind(gridPos, targetGrid, level);
                    //TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(path);
                    oldGridPos = Vector2.Zero;
                }
                if (path != null && (!gridPos.Equals(oldGridPos) || oldGridPos.Equals(Vector2.Zero)))
                {
                    oldGridPos = gridPos;

                    // Move towards waypoint
                    while (path.Waypoints.Count > 0 && Vector2.Distance(gridPos, path.Waypoints[0]) < WaypointMargin)
                    {
                        path.Waypoints.RemoveAt(0);
                    }
                    Vector2 move = Vector2.Zero;
                    if (path.Waypoints.Count > 0)
                    {
                        // Vector towards goal
                        move = (path.Waypoints[0] - gridPos) * PathWeighting;
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
                                // Get the Rectangle of the wall
                                Rectangle wall = level.WallRectangles[(int)adjacents[i].X, (int)adjacents[i].Y];
                                float gridWidth = level.CellWidth;
                                // Steer away from that bitch
                                Vector2 wallPos = new Vector2(wall.Center.X, wall.Center.Y);
                                Vector2 vec = (agent.TransformComponent.Position - wallPos);
                                float distance = vec.Length();
                                if (vec != Vector2.Zero)
                                    vec.Normalize();
                                vec.Y *= -1;
                                vec *= MathHelper.Lerp(0f, AvoidanceWeighting, (float)Math.Pow(gridWidth / distance, AvoidancePower));
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

        public void Update(GameTime gameTime)
        {
            if (agent.Enabled)
            {
                agent.ShootDirection = Vector2.Zero;
                // Check if target still active (if not, find something else to do)
                if (Target != null)
                {
                    if (Target is Collectible && ((Collectible)Target).Enabled == false)
                    {
                        OnDecisionTimer();
                        return;
                    }
                    else if (Target is Agent && ((Agent)Target).Enabled == false)
                    {
                        OnDecisionTimer();
                        return;
                    }
                    Vector2 aimingVec = Target.TransformComponent.Position - agent.TransformComponent.Position;
                    aimingVec.Y *= -1;
                    if (aimingVec.Length() > MinAttackDistance && aimingVec.Length() < MinAttackDistance)
                        agent.ShootDirection = aimingVec;
                }

                // Decrease timers
                decisionTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (decisionTimer < 0)
                {
                    OnDecisionTimer();
                }
                pathfindTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(pathfindTimer < 0)
                {
                    OnPathfindTimer();
                }
            }
        }

        private void OnDecisionTimer()
        {
            // Reset timer
            decisionTimer = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(MinDecisionCooldown, MaxDecisionCooldown);
            // Choose Action
            // Create list of valid targets
            List<ITransform> targets = TheyDontThinkItBeLikeItIsButItDo.WorldManager.GetActiveTransforms();
            int index = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, targets.Count);
            Target = targets[index];
        }

        private void OnPathfindTimer()
        {
            // Reset timer
            pathfindTimer = PathfindCooldown;
            // Recalculate a path
            Level level = TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level;
            Vector2 gridPos = new Vector2(level.GetGridX(agent.TransformComponent.Position.X), level.GetGridY(agent.TransformComponent.Position.Y));

            if (Target != null)
            {
                // Uncomment these lines to draw path for AI!
                //TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(path);
                Vector2 targetGrid = new Vector2(level.GetGridX(Target.TransformComponent.Position.X), level.GetGridY(Target.TransformComponent.Position.Y));
                path = Path.pathfind(gridPos, targetGrid, level);
                //TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(path);
                oldGridPos = Vector2.Zero;
            }
        }

        public void DamagedPlayer(Agent victim)
        {
            
        }

        public void KilledPlayer(Agent victim)
        {
            if (Target == victim)
            {
                OnDecisionTimer();
            }
        }

        public void BumpedPlayer(Agent victim)
        {
            // If AI is interruptible, chase new victim 20% of the time
            if (Target == victim)
            {
                if (TheyDontThinkItBeLikeItIsButItDo.Rand.NextDouble() < 0.2)
                {
                    OnDecisionTimer();
                }
            }
            else if (TheyDontThinkItBeLikeItIsButItDo.Rand.NextDouble() < 0.2)
            {
                // Reset timer
                decisionTimer = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(MinDecisionCooldown, MaxDecisionCooldown);
                Target = victim;
            }
        }

        public void CollectedCollectible()
        {
            OnDecisionTimer();
        }
    }
}
