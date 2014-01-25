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
using GGJ2014.Levels;

namespace GGJ2014.GameObjects
{
    public class Agent : IMovement, IDraw, IUpdate, IDynamic
    {

        public IController Controller { set; private get; }
        private const float RevealDuration = 0.5f;
        private float revealTimer;
        private TransformComponent transformComponent;
        private MovementComponent movementComponent;
        private const float BaseSize = 10;
        private const float BaseSpeed = 20;
        private float size;
        public float Speed { get; set; }
        public TransformComponent TransformComponent { get { return this.transformComponent; } set { this.transformComponent = value; } }
        public MovementComponent MovementComponent { get { return this.movementComponent; } set { this.movementComponent = value; } }
        private float hitpoints = 100;
        private float fireRate = 20f;
        private const float BurstDuration = 0.5f;
        private const float DashDuration = 0.10f;
        private const float BurstCooldown = 0.8f;
        private const float DashCooldown = 1f;
        private const float DashMultiplier = 6f;
        private float burstTimer = 0f;
        private float dashTimer = 0f;
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
        private const float SprayAngle = MathHelper.Pi / 18;
        public bool Enabled { get; set; }

        private const float RespawnDuration = 2f;
        private float spawnTimer = RespawnDuration;

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
            this.Speed = TheyDontThinkItBeLikeItIsButItDo.Scale * Agent.BaseSpeed;
            this.size = TheyDontThinkItBeLikeItIsButItDo.Scale * Agent.BaseSize;
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), (int)this.size, (int)this.size, ZIndex.Player);
            this.xPenetrations = new List<float>();
            this.yPenetrations = new List<float>();
            this.possibleRectangles = new List<Rectangle>();
            this.Enabled = true;
        }

        public void Update(GameTime gameTime)
        {
            if (hitpoints <= 0)
            {
                Enabled = false;
            }
            if (!Enabled)
            {
                Enabled = false;
                // Count down spawn timer
                spawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (spawnTimer <= 0)
                {
                    spawnTimer = RespawnDuration;
                    Spawn();
                }
            }
            else // Alive, do update!
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
                this.movementComponent.Velocity += this.DesiredMovementDirection * Speed;

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
                            float angle = (float)Math.Atan2(this.ShootDirection.Y, this.ShootDirection.X);
                            float variation = (float)(TheyDontThinkItBeLikeItIsButItDo.Rand.NextDouble() - 0.5f) * Agent.SprayAngle;
                            angle += variation;
                            this.ShootDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                            this.ShootDirection.Normalize();
                            this.lastShootingDirection = this.ShootDirection;
                        }
                        else
                        {
                            this.ShootDirection = this.lastShootingDirection;
                        }
                        // shoot in the given direction
                        TheyDontThinkItBeLikeItIsButItDo.WorldManager.BulletPool.createBullet(this.transformComponent.Position, Vector2.Normalize(this.ShootDirection), this, this.movementComponent.Velocity);

                        //Play shooting sound
                        Microsoft.Xna.Framework.Audio.Cue shootCue = TheyDontThinkItBeLikeItIsButItDo.AudioManager.LoadCue("shoot");
                        TheyDontThinkItBeLikeItIsButItDo.AudioManager.PlayCue(ref shootCue, true);

                        timeLastFired = (float)gameTime.TotalGameTime.TotalSeconds;
                    }

                    // increase burst timer
                    this.burstTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (this.burstTimer >= Agent.BurstDuration)
                    {
                        this.burstTimer = Agent.BurstCooldown;
                        this.firing = false;
                    }
                }
                else
                {
                    this.burstTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (this.burstTimer < 0)
                    {
                        // Have now cooled down from shooting or from dash
                        this.burstTimer = 0;
                    }
                    // Dash duration timer!
                    this.dashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (this.dashTimer < 0)
                    {
                        this.dashTimer = 0;
                        Speed = BaseSpeed;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Enabled)
            {
                this.Sprite.Tint = Color.Lerp(Color.White, this.Color, this.revealTimer / Agent.RevealDuration);
                this.revealTimer = MathHelper.Clamp(this.revealTimer, 0, this.revealTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
                this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
            }
        }

        public void Spawn()
        {
            // Get suitable spawn point from WorldManager
            Vector2 spawn = TheyDontThinkItBeLikeItIsButItDo.WorldManager.FindSpawnPoint();
            // Reset stuff
            this.movementComponent.Velocity = Vector2.Zero;
            this.transformComponent.Position = spawn;
            this.hitpoints = 100;
            this.revealTimer = 0;
            this.Enabled = true;
            this.firing = false;
        }

        public void Dash()
        {
            if (burstTimer == 0)
            {
                firing = false;
                Speed = BaseSpeed * DashMultiplier;
                burstTimer = Agent.DashCooldown;
                dashTimer = Agent.DashDuration;
            }
        }

        public void HandleMapCollisions(Level level)
        {
            if (Enabled)
            {
                // if the player has moved
                if (this.transformComponent.Position != this.movementComponent.LastPosition)
                {
                    this.xPenetrations.Clear();
                    this.yPenetrations.Clear();
                    this.possibleRectangles.Clear();

                    // TESTING
                    //this.possibleRectangles.Add(new Rectangle(200, 200, 100, 100));

                    level.GetPossibleRectangles(this.possibleRectangles, this.transformComponent.Position, this.movementComponent.LastPosition, this.CollisionRectangle);

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
                            // this.movementComponent.LastPosition = this.transformComponent.Position;
                            this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitX * xPenetrations[0];
                            this.movementComponent.Velocity *= -Vector2.UnitX;
                            HandleMapCollisions(level);
                        }
                        else
                        {
                            this.yPenetrations.Sort();
                            // this.movementComponent.LastPosition = this.transformComponent.Position;
                            this.transformComponent.Position = this.transformComponent.Position - Vector2.UnitY * yPenetrations[0];
                            this.movementComponent.Velocity *= -Vector2.UnitY;
                            HandleMapCollisions(level);
                        }
                    }
                }
            }
        }

        public void HandleAgentCollisions(List<Agent> agents, int myIndex)
        {
            if (Enabled)
            {
                int numAgents = agents.Count;
                for (int i = myIndex + 1; i < numAgents; ++i)
                {
                    if (agents[i].Enabled && this.CollisionRectangle.Intersects(agents[i].CollisionRectangle))
                    {
                        // display colours
                        this.revealTimer = RevealDuration;
                        agents[i].revealTimer = RevealDuration;

                        this.Controller.BumpedPlayer(agents[i]);
                        agents[i].Controller.BumpedPlayer(this);
                    }
                }
            }
        }

        public void HandleBulletCollisions(List<Bullet> bullets)
        {
            if (Enabled)
            {
                Bullet bullet;
                int numBullets = bullets.Count;
                for (int i = 0; i < numBullets; ++i)
                {
                    bullet = bullets[i];
                    if (bullet.Owner.Color != this.Color &&
                        (bullet.CollisionRectangle.Intersects(this.CollisionRectangle) ||
                        this.CollisionRectangle.Contains(bullet.CollisionRectangle)))
                    {
                        // despawn that sucker
                        bullet.Lifespan = -1;

                        // take damage
                        this.hitpoints -= bullet.Damage;

                        // set the reveal timer (later to be replaced with blood)
                        this.revealTimer = Agent.RevealDuration;

                        bullet.Owner.Controller.DamagedPlayer(this);
                        //Play Hit Sound
                        Microsoft.Xna.Framework.Audio.Cue hitCue = TheyDontThinkItBeLikeItIsButItDo.AudioManager.LoadCue("hit");
                        TheyDontThinkItBeLikeItIsButItDo.AudioManager.PlayCue(ref hitCue, true);

                        if (this.hitpoints <= 0)
                        {
                            bullet.Owner.Controller.KilledPlayer(this);
                            // handle death

                            //Play Death Sound
                            Microsoft.Xna.Framework.Audio.Cue deathCue = TheyDontThinkItBeLikeItIsButItDo.AudioManager.LoadCue("death");
                            TheyDontThinkItBeLikeItIsButItDo.AudioManager.PlayCue(ref deathCue, true);
                        }
                    }
                }
            }
        }

        public void HandleCollectibleCollisions(List<Collectible> collectibles)
        {
            foreach (Collectible collectible in collectibles)
            {
                if (this.CollisionRectangle.Intersects(collectible.CollisionRectangle) ||
                    this.CollisionRectangle.Contains(collectible.CollisionRectangle) ||
                    collectible.CollisionRectangle.Contains(this.CollisionRectangle))
                {
                    if (collectible.Enabled && (collectible.color == this.Color || collectible.color == Color.White))
                    {
                        // add points
                        this.Controller.CollectedCollectible();

                        collectible.Remove();

                        //play sound
                        Microsoft.Xna.Framework.Audio.Cue collectCue = TheyDontThinkItBeLikeItIsButItDo.AudioManager.LoadCue("powerup");
                        TheyDontThinkItBeLikeItIsButItDo.AudioManager.PlayCue(ref collectCue, true);

                        //remove from world
                        TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(collectible);
                    }
                }
            }
        }
    }
}
