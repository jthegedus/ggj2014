
namespace GGJ2014.Graphics.SpriteAnimations
{
    using System;
    using Microsoft.Xna.Framework;
    using GGJ2014.Graphics;

    public class GrowShrinkEffect : SpriteEffect
    {
        private float rangeFromScale;

        public GrowShrinkEffect(float milliseconds, float rangeFromScale)
        {
            this.Duration = TimeSpan.FromMilliseconds(milliseconds);
            this.Permanent = true;

            this.rangeFromScale = rangeFromScale;
        }

        protected override void Update(Sprite sprite, float time)
        {
            if (time > 2.0f) this.ResetTime();
            if (time > 1.0f) time = 1.0f - (time - 1.0f);

            time -= 0.5f;
            time *= 2.0f;

            sprite.Zoom = 1f + (rangeFromScale * time);
        }
    }
}
