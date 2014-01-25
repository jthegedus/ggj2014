using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GGJ2014.Graphics;

namespace GGJ2014.Levels
{
    public class LevelLoader
    {
        public static Level LoadLevel(string filename)
        {
            Texture2D levelImage = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Levels/" + filename);
            int width = levelImage.Width;
            int height = levelImage.Height;
            Color[] textureColors = new Color[width * height];
            levelImage.GetData<Color>(textureColors); 
            Level level = new Level(width, height);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Color color = textureColors[y * width + x];
                    bool state = (color == Color.White) ? false : true;
                    level.setCell(x, y, state);
                    if (color == Color.Red)
                    {
                        level.addSpawn(x, y);
                    }
                }
            }
            level.UpdateWallRectangles();
            return level;
        }
    }
}
