﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using GGJ2014.Components;
using GGJ2014.Graphics;
using GGJ2014.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2014.GameObjects
{
    public class Collectible : IStatic, IDraw, IUpdate
    {
        private const float Duration = 10f;
        private float durationTimer;
        private const float BaseSize = 20;
        private float size;
        private const float FlickerDuration = 0.5f;
        private float flickerTimer;
        public Color color;
        private TransformComponent transformComponent;
        public Sprite Sprite { get; set; }

        public bool Enabled { get; set; }
        public float spawnTimer;
        public const int MinSpawnDuration = 2;
        public const int MaxSpawnDuration = 7;

        public Collectible(Vector2 position)
        {
            this.size = TheyDontThinkItBeLikeItIsButItDo.Scale * Collectible.BaseSize;
            this.transformComponent.Position = position;
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"),(int)this.size, (int)this.size);
            Reset();
            this.flickerTimer = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, 4);
            this.durationTimer = Collectible.Duration;
        }

        private void Reset()
        {
            // Reset both timers and the colours
            durationTimer = Duration;
            spawnTimer = (float)(TheyDontThinkItBeLikeItIsButItDo.Rand.NextDouble() * (MaxSpawnDuration - MinSpawnDuration) + MinSpawnDuration);
            spawnTimer = MathHelper.Clamp(spawnTimer, MinSpawnDuration, MaxSpawnDuration);

            Random rand = new Random();
            int col = rand.Next(1, 6);

            switch (col)
            {
                case 1:
                    color = Color.Red;
                    break;
                case 2:
                    color = Color.Green;
                    break;
                case 3:
                    color = Color.Blue;
                    break;
                case 4:
                    color = Color.Yellow;
                    break;
                case 5:
                    color = Color.White;
                    break;
                default:
                    break;
            }
        }

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

        public TransformComponent TransformComponent
        {
            get
            {
                return this.transformComponent;
            }
            set
            {
                this.transformComponent = value;
            }
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Enabled)
            {
                this.durationTimer -= elapsedTime;

                if (durationTimer <= 0)
                {
                    Remove();
                }
            }
            else
            {
                this.spawnTimer -= elapsedTime;
                if (spawnTimer <= 0)
                {
                    Enabled = true;
                    Reset();
                    Spawn();        
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (Enabled)
            {
                this.Sprite.Tint = this.color;
                this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
            }
        }

        private void Spawn()
        {
            // Get spawn point from WorldManager
            Vector2 spawn = TheyDontThinkItBeLikeItIsButItDo.WorldManager.FindCollectableSpawnPoint();
            this.transformComponent.Position = spawn;
        }

        public void Remove()
        {
            Enabled = false;
            Reset();
        }
    }
}
