using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Graphics;
using Microsoft.Xna.Framework;
using GGJ2014.Interfaces;
using GGJ2014.Components;
using Microsoft.Xna.Framework.Graphics;
using GGJ2014.Physics;
using GGJ2014.Levels;

namespace GGJ2014.GameObjects
{
    public class Bullet : IMovement, IDraw, IUpdate, IDynamic
    {
        // wall collisions
        // private List<float> xPenetrations;
        // private List<float> yPenetrations;
        private List<Rectangle> possibleRectangles;

        private const float BaseSize = 7.5f;
        private const float BaseSpeed = 300;
        private float size;
        private float speed;
        public Sprite Sprite { get; set; }
        private TransformComponent transformComponent;
        public TransformComponent TransformComponent { get { return this.transformComponent; } set{this.transformComponent = value; } }
        private MovementComponent movementComponent;
        public MovementComponent MovementComponent { get { return this.movementComponent; } set { this.movementComponent = value; } }

        public Agent Owner { get; set; }
        public Vector2 InitialPosition { get; set; }
        public float Lifespan { get; set; }
        public float Damage { get; set; }


        public Bullet() 
        {
            this.Damage = 20;
            // this.xPenetrations = new List<float>();
            // this.yPenetrations = new List<float>();
            this.possibleRectangles = new List<Rectangle>();
            this.speed = TheyDontThinkItBeLikeItIsButItDo.Scale * Bullet.BaseSpeed;
            this.size = TheyDontThinkItBeLikeItIsButItDo.Scale * Bullet.BaseSize;
        }

        public void Initialize(Vector2 ownerPosition, Vector2 direction, Agent owner, Vector2 initialVelocity)
        {
            Lifespan = 2000f;
            Owner = owner;
            InitialPosition = ownerPosition;
            this.transformComponent.Position = ownerPosition;
            this.movementComponent.Velocity = direction * speed + initialVelocity * 0f;
            // change sprite based on player?
            // Spritey things
            Texture2D baseball = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Baseball");
            this.Sprite = new Sprite(baseball, baseball.Width, baseball.Height, ZIndex.Player);
            this.Sprite.zIndex += 0.0001f;
            this.Sprite.Zoom = TheyDontThinkItBeLikeItIsButItDo.Scale * Bullet.BaseSize / baseball.Width;
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
            this.Sprite.Rotation += (float)(Math.PI * 2 * gameTime.ElapsedGameTime.TotalSeconds);
            this.transformComponent.Position += this.movementComponent.Velocity * new Vector2(1, -1) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.Lifespan -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    
        public void HandleMapCollisions(Level level)
        {
            // if the player has moved
            if (this.transformComponent.Position != this.movementComponent.LastPosition)
            {
                this.possibleRectangles.Clear();
                
                // fill list of possible rectangles from the level
                level.GetPossibleRectangles(this.possibleRectangles, this.transformComponent.Position, this.movementComponent.LastPosition, this.CollisionRectangle);

                foreach (Rectangle r in possibleRectangles)
                {
                    Vector2 penetration = CollisionSolver.SolveCollision(this, r);
                    if (penetration != Vector2.Zero)
                    {
                        this.Lifespan = -1;
                    }
                    
                    // if (penetration.X != 0)
                    // {
                    //     this.xPenetrations.Add(penetration.X);
                    // }
                    // if (penetration.Y != 0)
                    // {
                    //     this.yPenetrations.Add(penetration.Y);
                    // }
                }

                // if (xPenetrations.Count != 0 || yPenetrations.Count != 0)
                // {
                //     if (xPenetrations.Count >= yPenetrations.Count)
                //     {
                //         this.xPenetrations.Sort();
                //         this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitX * xPenetrations[0];
                //         this.movementComponent.Velocity *= -Vector2.UnitX;
                //         HandleMapCollisions();
                //     }
                //     else
                //     {
                //         this.yPenetrations.Sort();
                //         this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitY * yPenetrations[0];
                //         this.movementComponent.Velocity *= -Vector2.UnitY;
                //         HandleMapCollisions();
                //     }
                // }
            }
        }
        
        public Rectangle  CollisionRectangle
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
}
}
