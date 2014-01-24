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
        private float fireRate = 20f;
        private const float BurstDuration = 0.5f;
        private const float BurstCooldown = 1f;
        private float burstTimer = 0f;
        private float timeLastFired = 0.0f;
        private bool firing = false;
        private Vector2 lastShootingDirection;
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

            if (((this.ShootDirection != Vector2.Zero && this.burstTimer == 0f) || this.firing))
            {
                if (((float)gameTime.TotalGameTime.TotalSeconds - timeLastFired) > 1.0f / fireRate)
                {
                    // you can still shoot
                    firing = true;

                    if (this.ShootDirection != Vector2.Zero)
                    {
                        this.ShootDirection.Normalize();
                        this.lastShootingDirection = this.ShootDirection;
                    }
                    else
                    {
                        this.ShootDirection = this.lastShootingDirection;
                    }
                    // shoot in the given direction
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.BulletPool.createBullet(this.transformComponent.Position, Vector2.Normalize(this.ShootDirection), this.Color, this.movementComponent.Velocity);
                    timeLastFired = (float)gameTime.TotalGameTime.TotalSeconds;
                }

                // increase burst timer
                this.burstTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (this.burstTimer >= Agent.BurstDuration)
                {
                    this.burstTimer = Agent.BurstDuration;
                    this.firing = false;
                }
            }
            else
            {
                this.burstTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds / Agent.BurstCooldown;
                if (this.burstTimer < 0)
                {
                    this.burstTimer = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
        }
    }
}
