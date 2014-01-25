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
        private GroundType[] ground;
        public List<Rectangle> AgentSpawnRectangles { get; set; }
        public List<Rectangle> CollectableSpawnRectangles { get; set; }
        public Rectangle[,] WallRectangles { get; set; }

        private int Width { get; set; }
        private int Height { get; set; }

        private int CellWidth { get; set; }
        private int CellHeight { get; set; }

        private Sprite sprite;
        private Sprite dirtSprite;
        private Sprite grassSprite;
        private Sprite stoneSprite;
        private Sprite groundStoneSprite;

        private Sprite rockSprite;
        private Sprite bushSprite;
        
        public Level(int width, int height)
            : this(new bool[width * height], new GroundType[width * height], width, height)
        {
        }

        public Level(bool[] map, GroundType[] ground, int width, int height)
        {
            this.map = map;
            this.ground = ground;
            this.Width = width;
            this.Height = height;
            CellWidth = (int)(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / width);
            CellHeight = (int)(TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / height);

            // set sprites to textures
            this.sprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), CellWidth, CellHeight);
            Texture2D texture = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Dirt");
            this.grassSprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Grass"), texture.Width, texture.Height);
            this.stoneSprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Stone"), texture.Width, texture.Height);
            this.groundStoneSprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Stone"), texture.Width, texture.Height);
            this.rockSprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Rock"), texture.Width, texture.Height);
            this.bushSprite = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/Bush"), texture.Width, texture.Height);

            // determine scale
            this.dirtSprite = new Sprite(texture, texture.Width, texture.Height);
            float scale = (float)CellWidth / texture.Width;

            // set scale
            dirtSprite.Zoom = scale;
            grassSprite.Zoom = scale;
            stoneSprite.Zoom = scale;
            groundStoneSprite.Zoom = scale;
            rockSprite.Zoom = scale;
            bushSprite.Zoom = scale;

            // set z-depths
            grassSprite.zIndex = ZIndex.Ground;
            dirtSprite.zIndex = ZIndex.Ground;
            groundStoneSprite.zIndex = ZIndex.Ground;
            this.sprite.zIndex = ZIndex.Object;

            // spawn agents and collectables
            this.AgentSpawnRectangles = new List<Rectangle>();
            this.CollectableSpawnRectangles = new List<Rectangle>();
           
        }

        public bool getCell(int x, int y)
        {
            return this.map[y * this.Width + x];
        }

        public void setCell(int x, int y, bool state)
        {
            this.map[y * this.Width + x] = state;
        }

        public void addAgentSpawn(int x, int y)
        {
            AgentSpawnRectangles.Add(new Rectangle(x * CellWidth, y * CellHeight, CellWidth, CellHeight));
        }

        public void addCollectableSpawn(int x, int y)
        {
            CollectableSpawnRectangles.Add(new Rectangle(x * CellWidth, y * CellHeight, CellWidth, CellHeight));
        }

        public void setGround(int x, int y, GroundType gType)
        {
            this.ground[y * this.Width + x] = gType;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw ground
            for (int y = 0; y < Height; ++y)
            {
                this.dirtSprite.zIndex = ZIndex.Ground - (y * 0.001f);
                this.grassSprite.zIndex = ZIndex.Ground - (y * 0.001f);
                this.groundStoneSprite.zIndex = ZIndex.Ground - (y * 0.001f);
                for (int x = 0; x < Width; ++x)
                {
                    Vector2 pos = new Vector2(x * this.sprite.Width, y * this.sprite.Height);
                    if (this.ground[y * this.Width + x] == GroundType.DIRT)
                    {
                        this.dirtSprite.Draw(spriteBatch, pos, true);
                    }
                    else if (this.ground[y * this.Width + x] == GroundType.GRASS)
                    {
                        this.grassSprite.Draw(spriteBatch, pos, true);
                    }
                    else if (this.ground[y * this.Width + x] == GroundType.STONE)
                    {
                        this.groundStoneSprite.Draw(spriteBatch, pos, true);
                    }
                }
            }

            // Draw spawns
            //foreach (Rectangle spawn in AgentSpawnRectangles)
            //{
            //    this.sprite.Tint = Color.Red;
            //    this.sprite.Draw(spriteBatch, new Vector2(spawn.Center.X, spawn.Center.Y));
            //}
            //foreach (Rectangle spawn in CollectableSpawnRectangles)
            //{
            //    this.sprite.Tint = Color.Blue;
            //    this.sprite.Draw(spriteBatch, new Vector2(spawn.Center.X, spawn.Center.Y));
            //}


            // Draw collision objects
            int offset = this.stoneSprite.Height/5;
            this.stoneSprite.zIndex = ZIndex.Collision;
            for (int y = 0; y < Height; ++y)
            {
                this.stoneSprite.zIndex = ZIndex.Collision - (y * 0.001f);
                for (int x = 0; x < Width; ++x)
                {
                    if (WallRectangles[x, y] != null)
                    {
                        this.stoneSprite.Draw(spriteBatch, new Vector2(WallRectangles[x, y].Left, WallRectangles[x, y].Top - offset), true);
                    }
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

        public List<Rectangle> GetPossibleRectangles(List<Rectangle> rectangles, Vector2 position, Vector2 lastPosition, Rectangle collisionRectangle)
        {
            rectangles.Clear();

            Vector2 min = Vector2.Min(position, lastPosition);// / CellWidth;
            min.X -= collisionRectangle.Width / 2;
            min.Y -= collisionRectangle.Height / 2;
            Vector2 max = Vector2.Max(position, lastPosition);// / CellWidth;
            max.X += collisionRectangle.Width / 2;
            max.Y += collisionRectangle.Height / 2;

            int minX = (int)Math.Floor((min.X - CellWidth / 2) / CellWidth);
            int minY = (int)Math.Floor((min.Y - CellWidth / 2) / CellWidth);

            int maxX = (int)Math.Ceiling((max.X + CellWidth / 2) / CellWidth);
            int maxY = (int)Math.Ceiling((max.Y + CellWidth / 2) / CellWidth);

            for (int i = minX; i <= maxX; ++i)
            {
                for (int j = minY; j <= maxY; ++j)
                {
                    if (i >= 0 && i < WallRectangles.GetLength(0) && j >= 0 && j < WallRectangles.GetLength(1))
                    {
                        if (!getCell(i, j))
                        {
                            rectangles.Add(WallRectangles[i,j]);
                        }
                    }
                    else
                    {
                        rectangles.Add(new Rectangle(i * CellWidth - CellWidth / 2, j * CellWidth - CellWidth / 2, CellWidth, CellWidth));
                    }
                }
            }

            return rectangles;
        }
    }
}
