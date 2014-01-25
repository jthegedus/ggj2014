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
            Texture2D groundImage = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Levels/" + filename + "g");
            int width = levelImage.Width;
            int height = levelImage.Height;
            Color[] levelColors = new Color[width * height];
            Color[] groundColors = new Color[width * height];
            levelImage.GetData<Color>(levelColors);
            groundImage.GetData<Color>(groundColors);
            Level level = new Level(width, height);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    // Ground color check
                    Color color = groundColors[y * width + x];
                    if (color == Color.Green)
                    {
                        level.setGround(x, y, GroundType.GRASS);
                    }
                    else if (color == Color.Brown)
                    {
                        level.setGround(x, y, GroundType.DIRT);
                    }
                    else if(color == Color.White)
                    {
                        level.setGround(x, y, GroundType.STONE);
                    }

                    // Object color check
                    color = levelColors[y * width + x];
                    bool state = (color == Color.White) ? false : true;
                    level.setCell(x, y, state);
                    if (color == Color.Red)
                    {
                        level.addAgentSpawn(x, y);
                        Console.Out.WriteLine(color);
                    }
                    else if (color == Color.Blue)
                    {
                        level.addCollectableSpawn(x, y);
                    }
                }
            }
            level.UpdateWallRectangles();
            return level;
        }
    }
}
