using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GGJ2014.Interfaces;

namespace GGJ2014
{
    public class TextElement : IDraw
    {
        public String text { get; set; }
        public Vector2 pos { get; set; }
        public Color color { get; set; }
        public float zIndex { get; set; }
        public AnchorPoint AnchorPoint { get; set; }
        public Vector2 Scale { get; set; }

        public TextElement(String text, Vector2 pos, Color color, float zIndex)
        {
            this.text = text;
            this.pos = pos;
            this.color = color;
            this.zIndex = zIndex;
            this.Scale = new Vector2(1);
        }

        public void Draw(SpriteBatch spritebatch, GameTime gameTime)
        {
            spritebatch.DrawString(
                TheyDontThinkItBeLikeItIsButItDo.font, 
                this.text, 
                this.pos, 
                this.color, 
                0, 
                GetOrigin((int)TheyDontThinkItBeLikeItIsButItDo.font.MeasureString(this.text).X, (int)TheyDontThinkItBeLikeItIsButItDo.font.MeasureString(this.text).Y, this.AnchorPoint),
                this.Scale,
                SpriteEffects.None,
                zIndex);
        }

        public static Vector2 GetOrigin(int width, int height, AnchorPoint anchorPoint)
        {
            Vector2 origin = Vector2.Zero;

            switch (anchorPoint)
            {
                case AnchorPoint.Bottom:
                    origin = new Vector2(width / 2.0f, height);
                    break;
                case AnchorPoint.BottomLeft:
                    origin = new Vector2(0, height);
                    break;
                case AnchorPoint.BottomRight:
                    origin = new Vector2(width, height);
                    break;
                case AnchorPoint.Centre:
                    origin = new Vector2(width / 2.0f, height / 2.0f);
                    break;
                case AnchorPoint.Left:
                    origin = new Vector2(0, height / 2.0f);
                    break;
                case AnchorPoint.Right:
                    origin = new Vector2(width, height / 2.0f);
                    break;
                case AnchorPoint.Top:
                    origin = new Vector2(width / 2.0f, 0);
                    break;
                case AnchorPoint.TopLeft:
                    break;
                case AnchorPoint.TopRight:
                    origin = new Vector2(width, 0);
                    break;
            }

            return origin;
        }
    }
}
