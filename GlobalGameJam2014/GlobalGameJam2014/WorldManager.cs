using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2014
{
    public class WorldManager
    {
        List<ITransform> TransformObjects { get; set; }
        List<IDraw> DrawObjects { get; set; }
        List<IUpdate> UpdateObjects { get; set; }
        List<IMovement> MovementObjects { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        public WorldManager()
        {
            this.TransformObjects = new List<ITransform>();
            this.DrawObjects = new List<IDraw>();
            this.UpdateObjects = new List<IUpdate>();
            this.MovementObjects = new List<IMovement>();
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
        }

        public void Update(GameTime gameTime)
        {
            foreach (IUpdate obj in UpdateObjects)
            {
                obj.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            this.SpriteBatch.Begin();
            foreach (IDraw obj in DrawObjects)
            {
                obj.Draw(this.SpriteBatch, gameTime);
            }

            this.SpriteBatch.End();
        }
    }
}
