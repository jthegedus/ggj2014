using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GGJ2014.Interfaces;
using GGJ2014.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2014.GameObjects
{
    public class Agent : IMovement, IDraw, IUpdate
    {
        private TransformComponent transformComponent;
        private MovementComponent movementComponent;
        private float speed;
        public TransformComponent TransformComponent { get { return this.transformComponent; } set { this.transformComponent = value; } }
        public MovementComponent MovementComponent { get { return this.movementComponent; } set { this.movementComponent = value; } }
        private float hitpoints = 100;
        private float fireRate = 2;
        public Color Color { get; set; }
        public Vector2 ShootDirection { get; set; }
        public Sprite Sprite { get; set; }
        public Vector2 DesiredMovementDirection { get; set; }

        public Agent()
        {
            this.speed = TheyDontThinkItBeLikeItIsButItDo.PlayerSpeed;
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), TheyDontThinkItBeLikeItIsButItDo.PlayerSize, TheyDontThinkItBeLikeItIsButItDo.PlayerSize);
        }

        public void Update(GameTime gameTime)
        {
            this.movementComponent.Velocity *= 0.9f;
            this.movementComponent.Velocity += this.DesiredMovementDirection * speed;
            this.transformComponent.Position += this.movementComponent.Velocity * new Vector2(1, -1) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.ShootDirection != Vector2.Zero)
            {
                // shoot in the given direction
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
        }
    }
}
