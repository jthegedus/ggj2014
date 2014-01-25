using System;
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
        private const float Duration = 10;
        private float durationTimer;
        private const float BaseSize = 20;
        private float size;
        private const float FlickerDuration = 0.5f;
        private float flickerTimer;
        public List<Color> Colours { get; set; }
        private TransformComponent transformComponent;
        public Sprite Sprite { get; set; }

        public Collectible(Vector2 position)
        {
            this.size = TheyDontThinkItBeLikeItIsButItDo.Scale * Collectible.BaseSize;
            this.transformComponent.Position = position;
            this.Sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"),(int)this.size, (int)this.size);
            this.Colours = new List<Color>();
            Colours.Add(Color.Red);
            Colours.Add(Color.Yellow);
            Colours.Add(Color.Green);
            Colours.Add(Color.Blue);
            this.flickerTimer = TheyDontThinkItBeLikeItIsButItDo.Rand.Next(0, 4);
            this.durationTimer = Collectible.Duration;
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
            this.durationTimer -= elapsedTime;

            if (durationTimer <= 0)
            {
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
            }
            else
            {
                this.flickerTimer += elapsedTime;
                if (this.flickerTimer >= Collectible.FlickerDuration * this.Colours.Count)
                {
                    this.flickerTimer -= Collectible.FlickerDuration * this.Colours.Count;
                }

                Color tint = Colours[(int)(flickerTimer / Collectible.FlickerDuration)];
                this.Sprite.Tint = tint;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Sprite.Draw(spriteBatch, this.transformComponent.Position);
        }
    }
}
