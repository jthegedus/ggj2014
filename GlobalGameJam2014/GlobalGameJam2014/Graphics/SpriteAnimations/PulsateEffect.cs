
namespace GGJ2014.Graphics.SpriteAnimations
{
    using System;
    using Microsoft.Xna.Framework;
    using GGJ2014.Graphics;

    public class PulsateEffect : SpriteEffect
    {
        private float rangeFromMainOpacity;
        private float mainAlpha;

        public PulsateEffect(float milliseconds, float rangeFromMainOpacity, float mainAlpha = 1.0f)
        {
            this.Duration = TimeSpan.FromMilliseconds(milliseconds);
            this.Permanent = true;

            this.rangeFromMainOpacity = rangeFromMainOpacity;
            this.mainAlpha = mainAlpha;
        }

        protected override void Update(Sprite sprite, float time)
        {
            if (time > 2.0f) this.ResetTime();
            if (time > 1.0f) time = 1.0f - (time - 1.0f);

            time -= 0.5f;
            time *= 2.0f;

            sprite.Alpha = MathHelper.Clamp(mainAlpha + rangeFromMainOpacity * time, 0f, 1f);
        }
    }
}
