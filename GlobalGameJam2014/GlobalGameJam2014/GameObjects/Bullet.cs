using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Graphics;
using Microsoft.Xna.Framework;
using GGJ2014.Interfaces;
using GGJ2014.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2014.GameObjects
{
    public class Bullet : IMovement, IDraw
    {
        public Sprite Sprite { get; set; }
        public TransformComponent TransformComponent { get; set; }
        public MovementComponent MovementComponent { get; set; }

        public Bullet(Vector2 direction, float speed, Color owner)
        {
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Sprite.Draw(spriteBatch, this.TransformComponent.Position);
        }
    }
}
