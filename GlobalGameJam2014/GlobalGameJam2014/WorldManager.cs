using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GGJ2014.Graphics;
using GGJ2014.Levels;
using GGJ2014.GameObjects;
using GGJ2014.Controllers;
using GGJ2014.Components;

namespace GGJ2014
{
    public class WorldManager
    {
        List<ITransform> TransformObjects { get; set; }
        List<IDraw> DrawObjects { get; set; }
        List<IUpdate> UpdateObjects { get; set; }
        List<IMovement> MovementObjects { get; set; }
        List<IStatic> StaticObjects { get; set; }
        List<IDynamic> DynamicObjects { get; set; }
        List<Agent> Agents { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        private Sprite s;
        private Level level;
        private BulletPool bulletPool;
        public BulletPool BulletPool { get { return this.bulletPool; } }
        private List<Object> objsToRemove;
        private List<Object> objsToAdd;
        public Level Level { get { return this.level; } }

        public WorldManager()
        {
            this.TransformObjects = new List<ITransform>();
            this.DrawObjects = new List<IDraw>();
            this.UpdateObjects = new List<IUpdate>();
            this.MovementObjects = new List<IMovement>();
            this.StaticObjects = new List<IStatic>();
            this.DynamicObjects = new List<IDynamic>();
            this.Agents = new List<Agent>();
            this.bulletPool = new BulletPool();
            this.objsToRemove = new List<Object>();
            this.objsToAdd = new List<Object>();
        }

        public void InitGame()
        {

            for (int i = 0; i < 4; ++i)
            {
                Agents.Add(new Agent());
                this.AddToWorld(Agents[i]);
            }

            PlayerController player1 = new PlayerController(PlayerIndex.One, Agents[0]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player1);
            PlayerController player2 = new PlayerController(PlayerIndex.Two, Agents[1]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player2);
            PlayerController player3 = new PlayerController(PlayerIndex.Three, Agents[2]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player3);
            PlayerController player4 = new PlayerController(PlayerIndex.Four, Agents[3]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player4);
            
            // Randomise dem colours, MAAAAAAAAAAHN
            List<Color> colors = new List<Color>();
            colors.Add(Color.Red);
            colors.Add(Color.Blue);
            colors.Add(Color.Yellow);
            colors.Add(Color.Green);

            // Do 10 random swaps on colors list
            for (int i = 0; i < 10; ++i)
            {
                int index = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, colors.Count);
                Color temp = colors[index];
                colors.RemoveAt(index);
                colors.Add(temp);
            }

            // Assign colours
            for (int i = 0; i < Agents.Count; ++i)
            {
                Agents[i].Color = colors[i];
            }

            // Load level
            this.level = LevelLoader.LoadLevel("level04");

            // Assign player positions based on first 4 spawn points
            List<Rectangle> spawns = this.Level.SpawnRectangles;
            TransformComponent tc = new TransformComponent();
            for (int i = 0; i < 4; ++i)
            {
                tc.Position = new Vector2(spawns[i].Center.X, spawns[i].Center.Y);
                Agents[i].TransformComponent = tc;
            }

            //tc.Position = new Vector2(50, 50);
            //agents[0].TransformComponent = tc;
            //tc.Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, 50);
            //agents[1].TransformComponent = tc;
            //tc.Position = new Vector2(50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50);
            //agents[2].TransformComponent = tc;
            //tc.Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50);
            //agents[3].TransformComponent = tc;
        }

        public void AddToWorld(Object obj)
        {
            this.objsToAdd.Add(obj);
        }

        public void AddObjsToLists()
        {
            if (objsToAdd.Count > 0)
            {
                foreach (Object obj in objsToAdd)
                {
                    if (obj is ITransform)
                    {
                        this.TransformObjects.Add(obj as ITransform);
                    }

                    if (obj is IDraw)
                    {
                        this.DrawObjects.Add(obj as IDraw);
                    }

                    if (obj is IUpdate)
                    {
                        this.UpdateObjects.Add(obj as IUpdate);
                    }

                    if (obj is IMovement)
                    {
                        this.MovementObjects.Add(obj as IMovement);
                    }

                    if (obj is IStatic)
                    {
                        this.StaticObjects.Add(obj as IStatic);
                    }

                    if (obj is IDynamic)
                    {
                        this.DynamicObjects.Add(obj as IDynamic);
                    }
                }
                objsToAdd.Clear();
            }
        }

        public void RemoveFromWorld(Object obj)
        {
            this.objsToRemove.Add(obj);
        }

        private void RemoveInactiveObjsFromLists()
        {
            if (objsToRemove.Count > 0)
            {
                foreach (Object obj in objsToRemove)
                {
                    if (obj is ITransform)
                    {
                        this.TransformObjects.Remove(obj as ITransform);
                    }

                    if (obj is IDraw)
                    {
                        this.DrawObjects.Remove(obj as IDraw);
                    }

                    if (obj is IUpdate)
                    {
                        this.UpdateObjects.Remove(obj as IUpdate);
                    }

                    if (obj is IMovement)
                    {
                        this.MovementObjects.Remove(obj as IMovement);
                    }

                    if (obj is IStatic)
                    {
                        this.StaticObjects.Remove(obj as IStatic);
                    }

                    if (obj is IDynamic)
                    {
                        this.DynamicObjects.Remove(obj as IDynamic);
                    }
                }
                this.objsToRemove.Clear();
            }
        }

        public void Update(GameTime gameTime)
        {
            AddObjsToLists();
            bulletPool.Update(gameTime);
            foreach (IUpdate obj in UpdateObjects)
            {
                obj.Update(gameTime);
            }
            RemoveInactiveObjsFromLists();
        }

        public void HandleCollisions()
        {
            foreach (IDynamic obj in this.DynamicObjects)
            {
                obj.HandleMapCollisions(level);
            }

            int numAgents = this.Agents.Count;
            for (int i = 0; i < numAgents; ++i)
            {
                this.Agents[i].HandleAgentCollisions(this.Agents, i);
                this.Agents[i].HandleBulletCollisions(this.bulletPool.Bullets);
            }
        }

        public void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Begin();

            if (this.Level != null)
                this.Level.Draw(this.SpriteBatch, gameTime);

            //if (s == null)
            //    s = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), 100, 100);
            //s.Draw(this.SpriteBatch, new Vector2(200, 200), true);

            foreach (IDraw obj in DrawObjects)
            {
                obj.Draw(this.SpriteBatch, gameTime);
            }

            this.SpriteBatch.End();
        }

        public Vector2 FindSpawnPoint()
        {
            List<Rectangle> spawns = Level.SpawnRectangles;
            float[] spawnsDistance = new float[spawns.Count];
            // Initially populate big distances
            for (int i = 0; i < spawnsDistance.Length; ++i)
            {
                spawnsDistance[i] = TheyDontThinkItBeLikeItIsButItDo.ScreenWidth;
            }
            
            // Find smallest distance for each spawn point
            for(int i = 0; i < spawns.Count; ++i)
            {
                // If agent is closer than last closest distance, store the distance
                foreach (Agent agent in Agents)
                {
                    Vector2 spawnVec = new Vector2(spawns[i].Center.X, spawns[i].Center.Y);
                    float dist = Vector2.Distance(spawnVec, agent.TransformComponent.Position);
                    if (dist < spawnsDistance[i])
                    {
                        spawnsDistance[i] = dist; 
                    }
                }
            }

            // Find spawn point with largest distance
            int largestDistIndex = 0;
            for (int i = 0; i < spawnsDistance.Length; ++i)
            {
                if (spawnsDistance[i] > spawnsDistance[largestDistIndex])
                {
                    largestDistIndex = i;
                }
            }

            return new Vector2(spawns[largestDistIndex].Center.X, spawns[largestDistIndex].Center.Y);
        }
    }
}
