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
        public bool TargetIsPenalty { get; set; }

        public const float PenaltyDuration = 10f;
        public const float PenaltyDistance = 100;
        private float penaltyTimer = PenaltyDuration;

        private Vector2 oldGridPos = Vector2.Zero;

        public const float PathWeighting = 2f;
        public const float AvoidancePower = 4;
        public const float AvoidanceWeighting = 4f;
        public const float WaypointMargin = 1.2f;

        public const float HitpointsMultiplier = 2f;

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
            agent.Hitpoints *= HitpointsMultiplier;
        }

        public void HandleInput()
        {
            if (agent.Enabled)
            {
                Level level = TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level;
                Vector2 gridPos = new Vector2(level.GetGridX(agent.TransformComponent.Position.X), level.GetGridY(agent.TransformComponent.Position.Y));

                if (path != null && (!gridPos.Equals(oldGridPos) || oldGridPos.Equals(Vector2.Zero)))
                {
                    oldGridPos = gridPos;

                    // Move towards waypoint, clear waypoints when close enough
                    while (path.Waypoints.Count > 0 && Vector2.Distance(gridPos, path.Waypoints[0]) < WaypointMargin)
                    {
                        path.Waypoints.RemoveAt(0);
                    }
                    Vector2 move = Vector2.Zero;
                    // Check for end of path
                    if (path.Waypoints.Count <= 0)
                    {
                        OnPathFinished();
                    }
                    else
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
                                Rectangle me = agent.CollisionRectangle;
                                float meRadius = (me.Height * me.Height + me.Width * me.Width) / 2;
                                float wallRadius = (wall.Height * wall.Height + wall.Width * wall.Width) / 2;
                                float minClearanceSquared = meRadius + wallRadius;
                                // Steer away from that bitch
                                Vector2 wallPos = new Vector2(wall.Center.X, wall.Center.Y);
                                Vector2 vec = (agent.TransformComponent.Position - wallPos);
                                float distanceSquared = vec.LengthSquared();
                                if (vec != Vector2.Zero)
                                    vec.Normalize();
                                vec.Y *= -1;
                                vec *= MathHelper.Lerp(0f, AvoidanceWeighting, (float)Math.Pow(minClearanceSquared / distanceSquared, AvoidancePower));
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
                //agent.ShootDirection = Vector2.Zero;
                // Check if target still active (if not, find something else to do)
                if (Target != null && Target is Agent && ((Agent)Target).Enabled == false)
                {
                    OnDecisionTimer();
                    return;
                }
                if (Target != null && TargetIsPenalty)
                {
                    float distance = Vector2.Distance(agent.TransformComponent.Position, Target.TransformComponent.Position);
                    Console.Out.WriteLine("Distance: " + distance);
                    if (distance < PenaltyDistance && Target is Agent && ((Agent)Target).Controller is PlayerController)
                    {
                        ((PlayerController)((Agent)Target).Controller).Penalty = true;
                        SetPenaltyTarget(null);
                        OnDecisionTimer();
                    }
                    // Uncomment below for AI shooting (crazy accurate)
                    //Vector2 aimingVec = Target.TransformComponent.Position - agent.TransformComponent.Position;
                    //aimingVec.Y *= -1;
                    //if (aimingVec.Length() > MinAttackDistance && aimingVec.Length() < MaxAttackDistance)
                    //    agent.ShootDirection = aimingVec;
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
                if (TargetIsPenalty)
                {
                    if (Target is Agent && ((Agent)Target).Controller is PlayerController && ((PlayerController)((Agent)Target).Controller).Penalty == true)
                        penaltyTimer = 0;
                    penaltyTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if(penaltyTimer <= 0)
                    {
                        penaltyTimer = PenaltyDuration;
                        OnPenaltyTimerExpired();
                    }
                }
            }
        }

        private void OnDecisionTimer()
        {
            // If on a penalty chase, don't allow change of target
            if (Target == null || !TargetIsPenalty)
            {
                // Reset timer
                decisionTimer = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(MinDecisionCooldown, MaxDecisionCooldown);
                // Choose random player or collectible to walk towards
                List<ITransform> targets = TheyDontThinkItBeLikeItIsButItDo.WorldManager.GetActiveTransforms();
                int index = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, targets.Count);
                Target = targets[index];
            }
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

        private void OnPathFinished()
        {
            // If not chasing for penalty, choose random destination
            OnDecisionTimer();
        }

        private void OnPenaltyTimerExpired()
        {
            SetPenaltyTarget(null);
            OnDecisionTimer();
        }

        public void SetPenaltyTarget(PlayerController pc)
        {
            Target = (pc != null) ? pc.Agent : null;
            TargetIsPenalty = (pc != null);
            agent.Visible = (pc != null);
        }

        public bool ConsiderNewTarget(PlayerController pc)
        {
            // This guy shot me, should I chase him?
            if (Target == null || !TargetIsPenalty)
            {
                SetPenaltyTarget(pc);
                return true;
            }
            return false;
        }

        public void DamagedPlayer(Agent victim)
        {
            // Do nothing
        }

        public void KilledPlayer(Agent victim)
        {
            // Do nothing
        }

        public void BumpedPlayer(Agent victim)
        {
            // Do nothing
        }

        public void CollectedCollectible()
        {
            // Do nothing
        }

        public void Died()
        {
            // Clear any penalty chase
            SetPenaltyTarget(null);
        }

        public void Spawned()
        {
            agent.Hitpoints *= 2;
        }
    }
}
