using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GGJ2014.Interfaces;

namespace GGJ2014.Graphics
{
    public class FadingTextElement : TextElement, IUpdate, IDraw
    {
        public float Duration { get; set; }
        private float timer;
        public float StartingAlpha { get; set; }
        public float EndingAlpha { get; set; }

        public FadingTextElement(String text, Vector2 pos, Color color, float zIndex, float duration, float startingAlpha, float endingAlpha) : base(text, pos, color, zIndex)
        {
            this.StartingAlpha = 1;
            this.EndingAlpha = 0;
            this.Duration = duration;
            this.StartingAlpha = startingAlpha;
            this.EndingAlpha = endingAlpha;
            timer = duration;
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.timer -= elapsedTime;
            this.color = new Color(this.color.R, this.color.G, this.color.B, MathHelper.Lerp(EndingAlpha, StartingAlpha, this.timer / this.Duration));
            this.pos -= Vector2.UnitY * 100 * elapsedTime;
            if (this.timer <= 0)
            {
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
            }
        }
    }
}
