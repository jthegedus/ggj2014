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


        public WorldManager()
        {
            this.TransformObjects = new List<ITransform>();
            this.DrawObjects = new List<IDraw>();
            this.UpdateObjects = new List<IUpdate>();
            this.MovementObjects = new List<IMovement>();
            this.StaticObjects = new List<IStatic>();
            this.DynamicObjects = new List<IDynamic>();
            this.Agents = new List<Agent>();
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


            Agents[0].Color = Color.Red;
            Agents[1].Color = Color.Blue;
            Agents[2].Color = Color.Yellow;
            Agents[3].Color = Color.Green;

            TransformComponent tc = new TransformComponent();
            tc.Position = new Vector2(50, 50);
            Agents[0].TransformComponent = tc;
            tc.Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, 50);
            Agents[1].TransformComponent = tc;
            tc.Position = new Vector2(50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50);
            Agents[2].TransformComponent = tc;
            tc.Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50);
            Agents[3].TransformComponent = tc;

            // this.level = LevelLoader.LoadLevel("level04");
        }

        public void AddToWorld(Object obj)
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

        public void Update(GameTime gameTime)
        {
            foreach (IUpdate obj in UpdateObjects)
            {
                obj.Update(gameTime);
            }
        }

        public void HandleCollisions()
        {
            foreach (IDynamic obj in this.DynamicObjects)
            {
                obj.HandleMapCollisions();
            }

            int numAgents = this.Agents.Count;
            for (int i = 0; i < numAgents; ++i)
            {
                this.Agents[i].HandleAgentCollisions(this.Agents, i);
            }
        }

        public void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Begin();
            
            if (s == null)
                s = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), 100, 100);
            s.Draw(this.SpriteBatch, new Vector2(200, 200), true);

            foreach (IDraw obj in DrawObjects)
            {
                obj.Draw(this.SpriteBatch, gameTime);
            }

            if (this.level != null)
                this.level.Draw(this.SpriteBatch, gameTime);
            this.SpriteBatch.End();
        }
    }
}
