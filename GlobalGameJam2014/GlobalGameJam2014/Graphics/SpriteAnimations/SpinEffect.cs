
namespace GGJ2014.Graphics.SpriteAnimations
{
    using System;
    using GGJ2014.Graphics;
    using Microsoft.Xna.Framework;

    public class SpinEffect : SpriteEffect
    {
        public SpinEffect(float milliseconds)
        {
            this.Duration = TimeSpan.FromMilliseconds(milliseconds);
            this.Permanent = true;
        }

        protected override void Update(Sprite sprite, float time)
        {
            if (time > 1.0f) this.ResetTime();
            sprite.Rotation = MathHelper.WrapAngle(time * MathHelper.TwoPi);
        }
    }
}
