
namespace GGJ2014.Graphics
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Sprite
    {
        public Color Tint { get; set; }
        public float Alpha { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }
        public Texture2D Texture2D { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float zIndex { get; set; }

        public Sprite(Texture2D texture, int width, int height, float zIndex)
        {
            this.Texture2D = texture;
            this.Width = width;
            this.Height = height;
            this.Tint = Color.White;
            this.Zoom = 1.0f;

            this.Alpha = 1f;

            if (zIndex < 0)
                this.zIndex = 0f;
            else if (zIndex > 1)
                this.zIndex = 1f;
            else
                this.zIndex = zIndex;
        }

        public Sprite(Sprite sprite)
            : this(sprite.Texture2D, sprite.Width, sprite.Height, 0f)
        {
        }

        public Sprite(Texture2D texture, int width, int height)
            : this(texture, width, height, 0f)
        {
        }
    
        public void Draw(SpriteBatch spriteBatch, Vector2 point, bool fromTopLeft = false)
        {
            Color multipliedTint = this.Tint * this.Alpha;
            spriteBatch.Draw(
                this.Texture2D,
                point,
                this.GetSourceRectangle(),
                multipliedTint,
                this.Rotation,
                this.GetOrigin(fromTopLeft),
                this.Zoom,
                SpriteEffects.None,
                this.zIndex);
        }

        private Vector2 GetOrigin(bool fromTopLeft)
        {
            if (fromTopLeft) return Vector2.Zero;
            else return new Vector2(this.Width / 2f, this.Height / 2f);
        }

        private Rectangle? GetSourceRectangle()
        {
            return new Rectangle(0, 0, this.Width, this.Height);
        }
    }
}
