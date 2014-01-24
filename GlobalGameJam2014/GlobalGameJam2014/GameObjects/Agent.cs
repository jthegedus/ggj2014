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
using GGJ2014.Physics;

namespace GGJ2014.GameObjects
{
    public class Agent : IMovement, IDraw, IUpdate, IDynamic
    {
        private const float RevealDuration = 0.5f;
        private float revealTimer;
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
            this.movementComponent.Velocity *= 54f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.movementComponent.Velocity.LengthSquared() < 0.5f)
            {
                this.movementComponent.Velocity = Vector2.Zero;
            }

            // add the current impulse to the velocity
            this.movementComponent.Velocity += this.DesiredMovementDirection * speed;

            // apply the current velocity to the position
            this.transformComponent.Position += this.movementComponent.Velocity * new Vector2(1, -1) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // shoot
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
            this.Sprite.Tint = Color.Lerp(Color.White, this.Color, this.revealTimer / Agent.RevealDuration);
            this.revealTimer = MathHelper.Clamp(this.revealTimer, 0, this.revealTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
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
                this.possibleRectangles.Add(new Rectangle(200, 200, 100, 100));
                
                // fill list of possible rectangles from the level

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
                        this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitX * xPenetrations[0];
                        this.movementComponent.Velocity *= -Vector2.UnitX;
                        HandleMapCollisions();
                    }
                    else
                    {
                        this.yPenetrations.Sort();
                        this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitY * yPenetrations[0];
                        this.movementComponent.Velocity *= -Vector2.UnitY;
                        HandleMapCollisions();
                    }
                }
            }
        }

        public void HandleAgentCollisions(List<Agent> agents, int myIndex)
        {
            int numAgents = agents.Count;
            for (int i = myIndex + 1; i < numAgents; ++i)
            {
                if (this.CollisionRectangle.Intersects(agents[i].CollisionRectangle))
                {
                    // display colours
                    this.revealTimer = RevealDuration;
                    agents[i].revealTimer = RevealDuration;
                }
            }
        }

        public void HandleBulletCollisions()
        {
        }

        public void HandleCollectibleCollisions()
        {
        }
    }
}
