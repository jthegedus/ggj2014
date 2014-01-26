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
using GGJ2014.AI;
using Microsoft.Xna.Framework.Input;

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
        private Level level;
        private BulletPool bulletPool;
        public BulletPool BulletPool { get { return this.bulletPool; } }
        private List<Object> objsToRemove;
        private List<Object> objsToAdd;
        public Level Level { get { return this.level; } }
        private const float TimeLimit = 70;
        public static int NumberOfPlayers = 4;
        public static int NumberOfAI = 2;
        public static int NumberOfCollectibles = 4;

        public int DisplayedTime { get; set; }
        public int LastDisplayedTime { get; set; }
        public float GameTimer { get; private set; }
        public int LastLevel { get; private set; }

        public WorldManager()
        {
            this.DisplayedTime = (int)GameTimer;
            this.LastDisplayedTime = (int)GameTimer;
            this.GameTimer = TimeLimit;
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
            this.LastLevel = -1;
        }

        public void InitGame()
        {
            this.GameTimer = TimeLimit;
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.ClearLists();
            TheyDontThinkItBeLikeItIsButItDo.GameUI.Init();

            // Load level
            // WARNING!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! - ONLY USE LEVELS THAT HAVE A levelXX.png and a levelXXg.png (10 -> 16)
            // Randomise the level to load
            int levelToLoad = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(10, 17);
            while (levelToLoad == this.LastLevel)
            {
                levelToLoad = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(10, 17);
            }
            // SET LEVEL HERE FOR DEBUG
            //levelToLoad = 13;
            this.level = LevelLoader.LoadLevel("level" + levelToLoad);
            this.LastLevel = levelToLoad;

            // Decide number of players
            NumberOfPlayers = 0;
            if(GamePad.GetState(PlayerIndex.One).IsConnected) NumberOfPlayers++;
            if(GamePad.GetState(PlayerIndex.Two).IsConnected) NumberOfPlayers++;
            if(GamePad.GetState(PlayerIndex.Three).IsConnected) NumberOfPlayers++;
            if(GamePad.GetState(PlayerIndex.Four).IsConnected) NumberOfPlayers++;

            for (int i = 0; i < NumberOfPlayers + NumberOfAI; ++i)
            {
                Agents.Add(new Agent());
                this.AddToWorld(Agents[i]);
            }

            // Randomise dem colours, MAAAAAAAAAAHN
            List<Color> colors = new List<Color>();
            for (int i = 0; i < NumberOfPlayers; ++i)
            {
                colors.Add(PlayerController.Colors[i]);
            }

            // Do 10 random swaps on colors list
            for (int i = 0; i < 10; ++i)
            {
                int index = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, colors.Count);
                Color temp = colors[index];
                colors.RemoveAt(index);
                colors.Add(temp);
            }

            // Assign colours
            for (int i = 0; i < NumberOfPlayers; ++i)
            {
                Agents[i].Color = colors[i];
                PlayerController player = new PlayerController(PlayerIndex.One, Agents[i]);
                TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player);
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(player);
            }

            // AI Controllers
            for (int i = NumberOfPlayers; i < NumberOfPlayers + NumberOfAI; ++i)
            {
                AIController ai = new AIController(Agents[i]);
                TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(ai);
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(ai);
                Agents[i].Controller = ai;
            }

            // Add collectibles
            for (int i = 0; i < NumberOfCollectibles; ++i)
            {
                Collectibles.Add(new Collectible(new Vector2(0, 0)));
                this.AddToWorld(Collectibles[i]);
            }

            // Assign player positions based on first 4 spawn points
            List<Rectangle> spawns = this.Level.AgentSpawnRectangles;
            for (int i = 0; i < NumberOfPlayers+NumberOfAI; ++i)
            {
                Agents[i].Spawn();
            }

            TheyDontThinkItBeLikeItIsButItDo.GameUI.ShowUI();
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.Pregame;
        }

        public void AddToWorld(Object obj)
        {
            this.objsToAdd.Add(obj);
        }

        public void ClearLists()
        {
            this.TransformObjects.Clear();
            this.UpdateObjects.Clear();
            this.DrawObjects.Clear();
            this.StaticObjects.Clear();
            this.DynamicObjects.Clear();
            this.Agents.Clear();
            this.Collectibles.Clear();
            this.MovementObjects.Clear();
            this.objsToAdd.Clear();
            this.objsToRemove.Clear();
            this.bulletPool = new BulletPool();
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
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.GamePlaying)
            {
                this.GameTimer -= elapsedTime;
                this.DisplayedTime = (int)Math.Ceiling(this.GameTimer);
                if (this.DisplayedTime != this.LastDisplayedTime)
                {
                    TheyDontThinkItBeLikeItIsButItDo.GameUI.GameTimer.text = this.DisplayedTime.ToString();
                    this.LastDisplayedTime = this.DisplayedTime;
                }


                if (this.GameTimer <= 0)
                {
                    TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.GameEnded;
                    this.ClearLists();
                    TheyDontThinkItBeLikeItIsButItDo.EndMenu.ShowMenu();
                    TheyDontThinkItBeLikeItIsButItDo.GameUI.ShowUI();
                }
            }

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
            this.SpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied);

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

        // Attempts to find the nearest AI to the player who isn't chasing someone and gets them to chase the player
        public void SetPenaltyChase(PlayerController pc)
        {
            float shortestDistance = TheyDontThinkItBeLikeItIsButItDo.ScreenWidth;
            AIController closestAi = null;
            foreach (Agent agent in Agents)
            {
                if (agent.Enabled && agent.Controller is AIController && !((AIController)agent.Controller).TargetIsPenalty)
                {
                    float distance = Vector2.Distance(agent.TransformComponent.Position, pc.Agent.TransformComponent.Position);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestAi = ((AIController)agent.Controller);
                    }
                }
            }

            if (closestAi != null)
            {
                closestAi.SetPenaltyTarget(pc);
            }
        }

        public List<ITransform> GetActiveTransforms()
        {
            List<ITransform> list = new List<ITransform>();
            foreach(ITransform t in TransformObjects)
            {
                if (t is Collectible && ((Collectible)t).Enabled)
                {
                    list.Add(t);
                }
                else if (t is Agent && ((Agent)t).Enabled)
                {
                    list.Add(t);
                }
            }
            return list;
        }

        public Vector2 FindSpawnPoint()
        {
            List<Rectangle> spawns = Level.AgentSpawnRectangles;
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

        public Vector2 FindCollectableSpawnPoint()
        {
            int tryCount = 0;
            do
            {
                ++tryCount;
                int spawnIndex = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, Level.CollectableSpawnRectangles.Count);
                Vector2 pos = new Vector2(Level.CollectableSpawnRectangles[spawnIndex].Center.X, Level.CollectableSpawnRectangles[spawnIndex].Center.Y);
                // Check distance from other collectables
                float shortestDist = TheyDontThinkItBeLikeItIsButItDo.ScreenWidth;
                foreach (Collectible c in Collectibles)
                {
                    float dist = Vector2.Distance(pos, c.TransformComponent.Position);
                    if (dist < shortestDist)
                        shortestDist = dist;
                }
                // If at least 64 away, it's suitable (won't duplicate spawn)
                if (shortestDist > 16f)
                    return pos;
            }
            while (tryCount < 10);
            throw new Exception("Unable to find spawn location for collectable!!");
        }
    }
}
