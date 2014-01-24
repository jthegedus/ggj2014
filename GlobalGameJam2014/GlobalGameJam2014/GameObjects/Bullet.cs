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
        public float Damage { get; set; }


        public Bullet() 
        {
            this.Damage = 20;
            // this.xPenetrations = new List<float>();
            // this.yPenetrations = new List<float>();
            this.possibleRectangles = new List<Rectangle>();
        }

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
