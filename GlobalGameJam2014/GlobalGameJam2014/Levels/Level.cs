using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GGJ2014.Levels
{
    public class Level
    {
        private bool[] map;
        public List<Rectangle> SpawnRectangles { get; set; }
        public Rectangle[,] WallRectangles { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Sprite sprite;

        private int CellWidth { get; set; }
        private int CellHeight { get; set; }

        public Level(int width, int height)
            : this(new bool[width * height], width, height)
        {
        }

        public Level(bool[] map, int width, int height)
        {
            this.map = map;
            this.Width = width;
            this.Height = height;
            CellWidth = (int)(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / width);
            CellHeight = (int)(TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / height);
            this.sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), CellWidth, CellHeight);
            this.SpawnRectangles = new List<Rectangle>();
        }

        public bool getCell(int x, int y)
        {
            return this.map[y * this.Width + x];
        }

        public void setCell(int x, int y, bool state)
        {
            this.map[y * this.Width + x] = state;
        }

        public void addSpawn(int x, int y)
        {
            SpawnRectangles.Add(new Rectangle(x * CellWidth, y * CellHeight, CellWidth, CellHeight));
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw walls
            this.sprite.Tint = Color.DarkGray;
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Vector2 pos = new Vector2((this.sprite.Width / 2) + x * this.sprite.Width, (this.sprite.Height / 2) + y * this.sprite.Height);
                    if (this.getCell(x, y) == false)
                    {
                        this.sprite.Draw(spriteBatch, pos);
                    }
                }
            }
            // Draw spawns
            foreach (Rectangle spawn in SpawnRectangles)
            {
                this.sprite.Tint = Color.Red;
                this.sprite.Draw(spriteBatch, new Vector2(spawn.Center.X, spawn.Center.Y));
            }

            // Draw rectangles
            this.sprite.Tint = Color.Blue;
            foreach (Rectangle wall in WallRectangles)
            {
                if (wall != null)
                {
                    this.sprite.Tint = Color.Blue;
                    this.sprite.Draw(spriteBatch, new Vector2(wall.Center.X, wall.Center.Y));
                }
            }
        }

        public void UpdateWallRectangles()
        {
            this.WallRectangles = new Rectangle[Width, Height];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (this.getCell(x, y) == false)
                    {
                        this.WallRectangles[x,y] = new Rectangle(x * sprite.Width, y * sprite.Height, sprite.Width, sprite.Height);
                    }
                }
            }
        }
    }
}
