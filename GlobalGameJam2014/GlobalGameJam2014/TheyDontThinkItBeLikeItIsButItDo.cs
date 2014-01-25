using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GGJ2014.GameObjects;
using GGJ2014.Controllers;
using GGJ2014.Components;
using GGJ2014.Graphics;

namespace GGJ2014
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TheyDontThinkItBeLikeItIsButItDo : Microsoft.Xna.Framework.Game
    {
        public static GameState Gamestate { get; set; }
        public static Menu Menu { get; set; }
        public static SpriteFont font { get; set; }
        public static float Scale { get; set; }
        public static float ScreenWidth { get; private set; }
        public static float ScreenHeight { get; private set; }
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static WorldManager WorldManager { get; set; }
        public static ContentManager ContentManager { get; set; }
        public static ControllerManager ControllerManager { get; set; }
        public static AudioManager AudioManager { get; set; }
        public static Random Rand { get; set; }

        public TheyDontThinkItBeLikeItIsButItDo()
        {
            graphics = new GraphicsDeviceManager(this);
            // graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            // graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 64 * 25;
            graphics.PreferredBackBufferHeight = 36 * 25;
            TheyDontThinkItBeLikeItIsButItDo.Scale = (float)graphics.PreferredBackBufferWidth / (64 * 15);
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;
            Content.RootDirectory = "Content";
            this.Window.Title = "They Don't Think It Be Like It Is, But It Do";
            TheyDontThinkItBeLikeItIsButItDo.Rand = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.MAINMENU;

            TheyDontThinkItBeLikeItIsButItDo.WorldManager = new WorldManager();
            TheyDontThinkItBeLikeItIsButItDo.ContentManager = this.Content;
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager = new ControllerManager();
            TheyDontThinkItBeLikeItIsButItDo.AudioManager = new AudioManager();
            WorldManager.InitGame();

            TheyDontThinkItBeLikeItIsButItDo.Menu = new Menu();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WorldManager.SpriteBatch = spriteBatch;
            Menu.spritebatch = spriteBatch;
            // TODO: use this.Content to load your game content here

            TheyDontThinkItBeLikeItIsButItDo.font = Content.Load<SpriteFont>("SpriteFonts/Arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.GAMEPLAYING)
            {
                TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Update();
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.Update(gameTime);
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.HandleCollisions();
            }
            else if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.MAINMENU)
            {
                Menu.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here

            if(TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.GAMEPLAYING)
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.Draw(gameTime);
            else if(TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.MAINMENU)
            {
                Menu.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
