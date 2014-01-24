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
        }

        public void InitGame()
        {
            List<Agent> agents = new List<Agent>();

            for (int i = 0; i < 4; ++i)
            {
                agents.Add(new Agent());
                this.AddToWorld(agents[i]);
            }

            PlayerController player1 = new PlayerController(PlayerIndex.One, agents[0]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player1);
            PlayerController player2 = new PlayerController(PlayerIndex.Two, agents[1]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player2);
            PlayerController player3 = new PlayerController(PlayerIndex.Three, agents[2]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player3);
            PlayerController player4 = new PlayerController(PlayerIndex.Four, agents[3]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player4);

            TransformComponent tc = new TransformComponent();
            tc.Position = new Vector2(50, 50);
            agents[0].TransformComponent = tc;
            tc.Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, 50);
            agents[1].TransformComponent = tc;
            tc.Position = new Vector2(50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50);
            agents[2].TransformComponent = tc;
            tc.Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50);
            agents[3].TransformComponent = tc;

            this.level = LevelLoader.LoadLevel("level04");
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
