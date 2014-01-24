using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GGJ2014.Graphics;

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

        public WorldManager()
        {
            this.TransformObjects = new List<ITransform>();
            this.DrawObjects = new List<IDraw>();
            this.UpdateObjects = new List<IUpdate>();
            this.MovementObjects = new List<IMovement>();
            this.StaticObjects = new List<IStatic>();
            this.DynamicObjects = new List<IDynamic>();
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

            this.SpriteBatch.End();
        }
    }
}
