
namespace GGJ2014.Graphics.SpriteAnimations
{
    using System;
    using Microsoft.Xna.Framework;
    using GGJ2014.Graphics;

    public abstract class SpriteEffect
    {
        public TimeSpan Duration { get; protected set; }
        protected TimeSpan TimeElapsed { get; private set; }
        protected bool Permanent { get; set; }
        private Delay delay = new Delay(0f);

        protected abstract void Update(Sprite sprite, float time);

        public void Update(Sprite sprite, GameTime gameTime)
        {
            if (!delay.Over())
            {
                delay.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds);
                return;
            }

            this.TimeElapsed += gameTime.ElapsedGameTime;
            this.Update(sprite, (float)(this.TimeElapsed.TotalMilliseconds / this.Duration.TotalMilliseconds));
        }

        public bool HasFinished()
        {
            return !Permanent && this.TimeElapsed > this.Duration;
        }

        public void DelayStart(float milliseconds)
        {
            this.delay = new Delay(milliseconds);
        }

        protected void ResetTime()
        {
            this.TimeElapsed = TimeSpan.Zero;
        }
    }
}
