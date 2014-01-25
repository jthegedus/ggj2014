using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;

namespace GGJ2014
{
    public class GameOverScreen : IDraw
    {
        public TextElement Winner { get; set; }
        public TextElement PlayAgain { get; set; }
        public TextElement QuitToMenu { get; set; }
        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
