using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ2014
{
    class TextElement
    {
        public String text { get; set; }
        public Vector2 pos { get; set; }
        public Color color { get; set; }
        public float zIndex { get; set; }

        public TextElement(String text, Vector2 pos, Color color, float zIndex)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
            this.zIndex = zIndex;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Begin();
            //zIndex logic goes here???
            spritebatch.DrawString(TheyDontThinkItBeLikeItIsButItDo.font, this.text, this.pos, this.color);

            spritebatch.End();
        }
    }
}
