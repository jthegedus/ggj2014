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

namespace GGJ2014
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TheyDontThinkItBeLikeItIsButItDo : Microsoft.Xna.Framework.Game
    {
        public static int PlayerSize { get; private set; }
        public static float PlayerSpeed { get; private set; }
        private const float PlayerScreenSizeRatio = 0.02f;
        private const float PlayerScreenSpeedRatio = 0.05f;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static WorldManager WorldManager { get; set; }
        public static ContentManager ContentManager { get; set; }
        public static ControllerManager ControllerManager { get; set; }

        public TheyDontThinkItBeLikeItIsButItDo()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            this.Window.Title = "They Don't Think It Be Like It Is, But It Do";
        }

        public void InitGame()
        {
            TheyDontThinkItBeLikeItIsButItDo.PlayerSize = (int)(TheyDontThinkItBeLikeItIsButItDo.PlayerScreenSizeRatio * this.graphics.PreferredBackBufferWidth);
            TheyDontThinkItBeLikeItIsButItDo.PlayerSpeed = TheyDontThinkItBeLikeItIsButItDo.PlayerScreenSpeedRatio * this.graphics.PreferredBackBufferWidth;
            List<Agent> agents = new List<Agent>();

            for (int i = 0; i < 4; ++i)
            {
                agents.Add(new Agent());
                WorldManager.AddToWorld(agents[i]);
            }

            PlayerController player1 = new PlayerController(PlayerIndex.One, agents[0]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player1);
            PlayerController player2 = new PlayerController(PlayerIndex.Two, agents[1]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player2);
            PlayerController player3 = new PlayerController(PlayerIndex.Three, agents[2]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player3);
            PlayerController player4 = new PlayerController(PlayerIndex.Four, agents[3]);
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.AddController(player4);

            TransformComponent tc = new TransformComponent();
            tc.Position = new Vector2(50, 50);
            agents[0].TransformComponent = tc;
            tc.Position = new Vector2(graphics.PreferredBackBufferWidth - 50, 50);
            agents[1].TransformComponent = tc;
            tc.Position = new Vector2(50, graphics.PreferredBackBufferHeight - 50);
            agents[2].TransformComponent = tc;
            tc.Position = new Vector2(graphics.PreferredBackBufferWidth - 50, graphics.PreferredBackBufferHeight - 50);
            agents[3].TransformComponent = tc;
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
            TheyDontThinkItBeLikeItIsButItDo.WorldManager = new WorldManager();
            TheyDontThinkItBeLikeItIsButItDo.ContentManager = this.Content;
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager = new ControllerManager();
            InitGame();
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
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Update();
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.Update(gameTime);
            // TODO: Add your update logic here

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