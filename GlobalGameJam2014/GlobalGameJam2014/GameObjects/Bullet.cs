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
    public class Bullet : IMovement, IDraw, IUpdate
    {
        public Sprite Sprite { get; set; }
        private TransformComponent transformComponent;
        public TransformComponent TransformComponent { get { return this.transformComponent; } set{this.transformComponent = value; } }
        private MovementComponent movementComponent;
        public MovementComponent MovementComponent { get { return this.movementComponent; } set { this.movementComponent = value; } }

        public Color Owner { get; set; }
        public Vector2 InitialPosition { get; set; }
        private const float Speed = 0.5f;
        public float Lifespan { get; set; }


        public Bullet() { }

        public void Initialize(Vector2 ownerPosition, Vector2 direction, Color owner)
        {
            Lifespan = 2000f;
            Owner = owner;
            InitialPosition = ownerPosition;
            this.transformComponent.Position = ownerPosition;
            this.movementComponent.Velocity = direction * Speed;
            // change sprite based on player?
            // Spritey things
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), TheyDontThinkItBeLikeItIsButItDo.PlayerSize, TheyDontThinkItBeLikeItIsButItDo.PlayerSize);
            this.Sprite.Tint = Color.Beige;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // draw :D
            this.Sprite.Draw(spriteBatch, this.TransformComponent.Position);
        }

        public void Update(GameTime gameTime)
        {
            // update position (Y)
            this.transformComponent.Position += this.movementComponent.Velocity * new Vector2(1, -1) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            this.Lifespan -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
