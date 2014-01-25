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
        public static Dictionary<PlayerIndex, GamePadState> CurrentGPS { get; private set; }
        public static Dictionary<PlayerIndex, GamePadState> LastGPS { get; private set; }
        public static GameUI GameUI { get; set; }
        public static GameState Gamestate { get; set; }
        public static Menu Menu { get; set; }
        public static EndMenu EndMenu { get; set; }
        public static SpriteFont font { get; set; }
        public static SpriteFont LargeFont { get; set; }
        public static float Scale { get; set; }
        public static float ScreenWidth { get; private set; }
        public static float ScreenHeight { get; private set; }
        GraphicsDeviceManager graphics;
        static SpriteBatch spriteBatch;
        public static WorldManager WorldManager { get; set; }
        public static ContentManager ContentManager { get; set; }
        public static ControllerManager ControllerManager { get; set; }
        public static AudioManager AudioManager { get; set; }
        public static Random Rand { get; set; }

        public TheyDontThinkItBeLikeItIsButItDo()
        {
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS = new Dictionary<PlayerIndex, GamePadState>();
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.One] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Two] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Three] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Four] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.LastGPS = new Dictionary<PlayerIndex, GamePadState>();
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.One] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.Two] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.Three] = new GamePadState();
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.Four] = new GamePadState();
            graphics = new GraphicsDeviceManager(this);
            // graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            // graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.IsFullScreen = true;
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
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.MainMenu;

            TheyDontThinkItBeLikeItIsButItDo.ContentManager = this.Content;
            TheyDontThinkItBeLikeItIsButItDo.WorldManager = new WorldManager();
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager = new ControllerManager();
            TheyDontThinkItBeLikeItIsButItDo.AudioManager = new AudioManager();

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
            // TODO: use this.Content to load your game content here

            TheyDontThinkItBeLikeItIsButItDo.font = Content.Load<SpriteFont>("SpriteFonts/Arial14");
            TheyDontThinkItBeLikeItIsButItDo.LargeFont = Content.Load<SpriteFont>("SpriteFonts/Arial64Bold");

            TheyDontThinkItBeLikeItIsButItDo.GameUI = new GameUI();
            TheyDontThinkItBeLikeItIsButItDo.Menu = new Menu();
            TheyDontThinkItBeLikeItIsButItDo.EndMenu = new EndMenu();
            TheyDontThinkItBeLikeItIsButItDo.Menu.ShowMenu();
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
            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.Quit)
            {
                this.Exit();
            }
            Button.ButtonChangedThisUpdate = false;
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.One] = GamePad.GetState(PlayerIndex.One);
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Two] = GamePad.GetState(PlayerIndex.Two);
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Three] = GamePad.GetState(PlayerIndex.Three);
            TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Four] = GamePad.GetState(PlayerIndex.Four);

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            TheyDontThinkItBeLikeItIsButItDo.WorldManager.Update(gameTime);

            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.GamePlaying)
            {
                TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Update();
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.HandleCollisions();
            }
            else if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.MainMenu)
            {
                Menu.Update(gameTime);
            }

            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.One] = TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.One]; 
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.Two] = TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Two]; 
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.Three] = TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Three]; 
            TheyDontThinkItBeLikeItIsButItDo.LastGPS[PlayerIndex.Four] = TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.Four]; 

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
            
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
