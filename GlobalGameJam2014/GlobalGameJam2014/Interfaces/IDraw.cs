using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GGJ2014.Interfaces
{
    public interface IDraw
    {
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
