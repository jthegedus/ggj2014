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
        private const float PlayerBulletSizeRatio = 0.4f;
        public Sprite Sprite { get; set; }
        private TransformComponent transformComponent;
        public TransformComponent TransformComponent { get { return this.transformComponent; } set{this.transformComponent = value; } }
        private MovementComponent movementComponent;
        public MovementComponent MovementComponent { get { return this.movementComponent; } set { this.movementComponent = value; } }

        public Color Owner { get; set; }
        public Vector2 InitialPosition { get; set; }
        private const float Speed = 500f;
        public float Lifespan { get; set; }


        public Bullet() { }

        public void Initialize(Vector2 ownerPosition, Vector2 direction, Color owner, Vector2 initialVelocity)
        {
            Lifespan = 2000f;
            Owner = owner;
            InitialPosition = ownerPosition;
            this.transformComponent.Position = ownerPosition;
            this.movementComponent.Velocity = direction * Speed + initialVelocity * 0f;
            // change sprite based on player?
            // Spritey things
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), (int)(TheyDontThinkItBeLikeItIsButItDo.PlayerSize * Bullet.PlayerBulletSizeRatio), (int)(TheyDontThinkItBeLikeItIsButItDo.PlayerSize * Bullet.PlayerBulletSizeRatio));
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
            this.transformComponent.Position += this.movementComponent.Velocity * new Vector2(1, -1) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.Lifespan -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
