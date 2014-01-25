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
using GGJ2014.Controllers;

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
        private const float BaseSpeed = 30;
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
        private const float DashMultiplier = 4f;
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

        // Sprites
        private Sprite sprCap;
        private Sprite sprRefCap;
        private Sprite sprArm;
        private bool isFeet1;
        private Sprite sprFeet1;
        private Sprite sprFeet2;
        private bool isGlove1;
        private Sprite sprGlove1;
        private Sprite sprGlove2;
        private Vector2 lastFacing = Vector2.UnitY;
        public const float BaseAnimDuration = 0.2f;
        private float animTimer = BaseAnimDuration;

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
                        (int)(this.sprCap.Width * 0.8f),
                        (int)(this.sprCap.Height * 0.8f)));
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

            // Load sprites (jesus)
            Texture2D texCap = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/Cap");
            sprCap = new Sprite(texCap, texCap.Width, texCap.Height, ZIndex.Player);
            Texture2D texRefCap = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/RefCap");
            sprRefCap = new Sprite(texRefCap, texRefCap.Width, texRefCap.Height, ZIndex.Player);
            sprRefCap.zIndex += 0.00001f;
            Texture2D texArm = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/Arm");
            sprArm = new Sprite(texArm, texArm.Width, texArm.Height, ZIndex.Player);
            Texture2D texGlove1 = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/Glove1");
            sprGlove1 = new Sprite(texGlove1, texGlove1.Width, texGlove1.Height, ZIndex.Player);
            Texture2D texGlove2 = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/Glove2");
            sprGlove2 = new Sprite(texGlove2, texGlove2.Width, texGlove2.Height, ZIndex.Player);
            Texture2D texFeet1 = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/Feet1");
            sprFeet1 = new Sprite(texFeet1, texFeet1.Width, texFeet1.Height, ZIndex.Player);
            Texture2D texFeet2 = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Player/Feet2");
            sprFeet2 = new Sprite(texFeet2, texFeet2.Width, texFeet2.Height, ZIndex.Player);
        }

        public void Update(GameTime gameTime)
        {
            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.GamePlaying)
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

                // Update Animation Stuff
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                animTimer -= elapsedTime;
                if (animTimer < 0)
                {
                    animTimer = BaseAnimDuration;
                    isGlove1 = !isGlove1;
                    isFeet1 = !isFeet1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Enabled)
            {
                // Get direction
                Vector2 direction = lastFacing;
                if (!DesiredMovementDirection.Equals(Vector2.Zero))
                    direction = DesiredMovementDirection;
                direction.Normalize();
                // Calculate rotation
                float rotation = (float)Math.Atan2(direction.Y, -direction.X) - MathHelper.PiOver2;
                // Set feet rotation (so they follow movement direction)
                this.sprFeet1.Rotation = rotation;
                this.sprFeet2.Rotation = rotation;

                // Check shoot direction
                if (!ShootDirection.Equals(Vector2.Zero))
                {
                    direction = ShootDirection;
                    //Recalculate rotation
                    rotation = (float)Math.Atan2(direction.Y, -direction.X) - MathHelper.PiOver2;
                }
                lastFacing = direction;

                this.sprCap.Tint = Color.Lerp(Color.White, this.Color, this.revealTimer / Agent.RevealDuration);
                this.revealTimer = MathHelper.Clamp(this.revealTimer, 0, this.revealTimer - (float)gameTime.ElapsedGameTime.TotalSeconds);
                this.sprCap.Rotation = rotation;
                this.sprCap.Draw(spriteBatch, this.transformComponent.Position);
                this.sprRefCap.Rotation = rotation;
                this.sprRefCap.Draw(spriteBatch, this.transformComponent.Position);
                if (firing)
                {
                    this.sprArm.Rotation = rotation;
                    this.sprArm.Draw(spriteBatch, this.transformComponent.Position);
                }
                if (isGlove1)
                {
                    this.sprGlove1.Rotation = rotation;
                    this.sprGlove1.Draw(spriteBatch, this.transformComponent.Position);
                }
                else
                {
                    this.sprGlove2.Rotation = rotation;
                    this.sprGlove2.Draw(spriteBatch, this.transformComponent.Position);
                }

                if (!DesiredMovementDirection.Equals(Vector2.Zero))
                {
                    // Draw feet
                    if (isFeet1)
                        this.sprFeet1.Draw(spriteBatch, this.transformComponent.Position);
                    else
                        this.sprFeet2.Draw(spriteBatch, this.transformComponent.Position);
                }
                //this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
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
            if (Controller != null && Controller is PlayerController)
            {
                ((PlayerController)Controller).OnAgentSpawn();
            }
        }

        public void Dash()
        {
            if (burstTimer == 0 || firing)
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
