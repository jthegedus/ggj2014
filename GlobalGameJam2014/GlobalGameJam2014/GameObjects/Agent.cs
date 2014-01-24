﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GGJ2014.Interfaces;
using GGJ2014.Components;
using Microsoft.Xna.Framework.Graphics;
using GGJ2014.Physics;

namespace GGJ2014.GameObjects
{
    public class Agent : IMovement, IDraw, IUpdate, IDynamic
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
        private List<float> xPenetrations;
        private List<float> yPenetrations;
        private List<Rectangle> possibleRectangles;
        public Rectangle CollisionRectangle
        {
            get
            {
                return GeometryUtility.GetAdjustedRectangle(
                    this.transformComponent.Position,
                    new Rectangle(
                        (int)this.transformComponent.Position.X,
                        (int)this.transformComponent.Position.Y,
                        this.Sprite.Width,
                        this.Sprite.Height));
            }
        }

        public Agent()
        {
            this.speed = TheyDontThinkItBeLikeItIsButItDo.PlayerSpeed;
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), TheyDontThinkItBeLikeItIsButItDo.PlayerSize, TheyDontThinkItBeLikeItIsButItDo.PlayerSize);
            this.xPenetrations = new List<float>();
            this.yPenetrations = new List<float>();
            this.possibleRectangles = new List<Rectangle>();
        }

        public void Update(GameTime gameTime)
        {
            // set the last position
            this.movementComponent.LastPosition = this.transformComponent.Position;

            // attenuate current velocity so the player slows down. Stop them if they're really close to not moving
            this.movementComponent.Velocity *= 0.9f;
            if (this.movementComponent.Velocity.LengthSquared() < 0.5f)
            {
                this.movementComponent.Velocity = Vector2.Zero;
            }

            // add the current impulse to the velocity
            this.movementComponent.Velocity += this.DesiredMovementDirection * speed;

            // apply the current velocity to the position
            this.transformComponent.Position += this.movementComponent.Velocity * new Vector2(1, -1) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // shoot
            if (this.ShootDirection != Vector2.Zero)
            {
                // shoot in the given direction
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
        }

        public void HandleMapCollisions()
        {
            // if the player has moved
            if (this.transformComponent.Position != this.movementComponent.LastPosition)
            {
                this.xPenetrations.Clear();
                this.yPenetrations.Clear();
                this.possibleRectangles.Clear();

                // TESTING
                //this.possibleRectangles.Add(new Rectangle(200, 200, 100, 100));

                // Get grid coordinates from level
                Rectangle[,] wallRectangles = TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level.WallRectangles;
                foreach (Rectangle wall in wallRectangles)
                {
                    if (wall != null)
                    {
                        this.possibleRectangles.Add(wall);
                    }
                }

                foreach (Rectangle r in possibleRectangles)
                {
                    Vector2 penetration = CollisionSolver.SolveCollision(this, r);
                    if (penetration.X != 0)
                    {
                        this.xPenetrations.Add(penetration.X);
                    }
                    if (penetration.Y != 0)
                    {
                        this.yPenetrations.Add(penetration.Y);
                    }
                }

                if (xPenetrations.Count != 0 || yPenetrations.Count != 0)
                {
                    if (xPenetrations.Count >= yPenetrations.Count)
                    {
                        this.xPenetrations.Sort();
                        this.movementComponent.LastPosition = this.transformComponent.Position;
                        this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitX * xPenetrations[0];
                        this.movementComponent.Velocity *= -Vector2.UnitX;
                        HandleMapCollisions();
                    }
                    else
                    {
                        this.yPenetrations.Sort();
                        this.movementComponent.LastPosition = this.transformComponent.Position;
                        this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitY * yPenetrations[0];
                        this.movementComponent.Velocity *= -Vector2.UnitY;
                        HandleMapCollisions();
                    }
                }
            }
        }

        public void HandleAgentCollisions()
        {
        }

        public void HandleBulletCollisions()
        {
        }

        public void HandleCollectibleCollisions()
        {
        }
    }
}
