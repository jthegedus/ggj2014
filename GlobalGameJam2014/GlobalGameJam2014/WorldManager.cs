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
        List<Collectible> Collectibles { get; set; }
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
            this.Collectibles = new List<Collectible>();
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

            Collectible c = new Collectible(new Vector2(300, 300));
            this.AddToWorld(c);

            Agents[0].Color = Color.Red;
            Agents[1].Color = Color.Blue;
            Agents[2].Color = Color.Yellow;
            Agents[3].Color = Color.Green;

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

                    if (obj is Collectible)
                    {
                        this.Collectibles.Add(obj as Collectible);
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

                    if (obj is Collectible)
                    {
                        this.Collectibles.Remove(obj as Collectible);
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
                this.Agents[i].HandleCollectibleCollisions(this.Collectibles);
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
    }
}
