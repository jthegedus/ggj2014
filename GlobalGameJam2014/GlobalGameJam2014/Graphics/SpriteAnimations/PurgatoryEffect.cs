
namespace GGJ2014.Graphics.SpriteAnimations
{
    using System;
    using Microsoft.Xna.Framework;
    using GGJ2014.Graphics;
using Microsoft.Xna.Framework.Audio;

    public class PurgatoryEffect : SpriteEffect
    {
        
        public PurgatoryEffect()
        {
            this.Duration = TimeSpan.FromMilliseconds(250);
        }

        protected override void Update(Sprite sprite, float time)
        {
            if (time < 0.25f || (time > 0.5f && time < 0.75f) || time > 0.9f)
            {
                sprite.Tint = Color.White;
            }
            else
            {
                sprite.Tint = Color.Black;
            }

            if (time > 0.9f)
            {
                sprite.Alpha = 0f;
            }
            else
            {
                sprite.Alpha = 1f;
            }
        }
    }
}
