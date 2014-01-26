using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GGJ2014.Interfaces;
using GGJ2014.Graphics;

namespace GGJ2014
{
    public class Menu : IDraw, IUpdate
    {
        public List<Button> Buttons { get; set; }
        public Sprite Splash { get; set; }
        public Sprite InstructionsScreen { get; set; }
        public Button Back { get; set; }
        public Button Back2 { get; set; }
        public Button Start { get; set; }
        public Button Instructions { get; set; }
        public Button Quit { get; set; }

        public Menu()
        {
            this.Buttons = new List<Button>();
            Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Backgrounds/titleScreen1080");
            this.Splash = new Sprite(img, img.Width, img.Height, 1) { AnchorPoint = AnchorPoint.TopLeft, Zoom = TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / img.Width };
            img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Backgrounds/gameRules1080");
            this.InstructionsScreen = new Sprite(img, img.Width, img.Height, 1) { AnchorPoint = GGJ2014.AnchorPoint.TopLeft, Zoom = TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / img.Width };

            Vector2 position = new Vector2(80, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 20);
            this.Start = new Button()
            {
                Position = position,
                Text = new TextElement("Start", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomLeft },
                RolloverText = new TextElement("Start", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomLeft },
                ClickText = new TextElement("Start", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomLeft },
                ClickDuration = 0.5f,
            };

            position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 20);
            this.Instructions = new Button()
            {
                Position = position,
                Text = new TextElement("Instructions", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.Bottom },
                RolloverText = new TextElement("Instructions", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.Bottom },
                ClickText = new TextElement("Instructions", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.Bottom },
                ClickDuration = 0.5f,
            };

            position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 80, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 20);
            this.Quit = new Button()
            {
                Position = position,
                Text = new TextElement("Quit", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomRight },
                RolloverText = new TextElement("Quit", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomRight },
                ClickText = new TextElement("Quit", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomRight },
                ClickDuration = 0.5f
            };

            position = new Vector2(50, 50);
            this.Back = new Button()
            {
                Position = position,
                Text = new TextElement("Back", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft },
                RolloverText = new TextElement("Back", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft },
                ClickText = new TextElement("Back", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft },
                ClickDuration = 0.5f
            };
            position = new Vector2(-1000);
            this.Back2 = new Button()
            {
                Position = position,
                Text = new TextElement("Back", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft },
                RolloverText = new TextElement("Back", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft },
                ClickText = new TextElement("Back", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft },
                ClickDuration = 0.5f,
                SelectButton = Microsoft.Xna.Framework.Input.Buttons.B
            };

            Start.Selected = true;

            Start.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.WorldManager.InitGame);
            Start.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.PregameScreen.Init);
            Start.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.PregameScreen.ShowMenu);
            Start.OnClick += new Action(this.HideMenu);
            Instructions.OnClick += new Action(this.GoToRules);
            Back.OnClick += new Action(this.BackToMenu);
            Back2.OnClick += new Action(this.BackToMenu);
            Quit.OnClick += new Action(this.QuitGame);

            Start.ButtonLeft = this.Quit;
            Start.ButtonRight = this.Instructions;

            Instructions.ButtonLeft = this.Start;
            Instructions.ButtonRight = this.Quit;

            Quit.ButtonLeft = this.Instructions;
            Quit.ButtonRight = this.Start;

            this.Buttons.Add(Start);
            this.Buttons.Add(Instructions);
            this.Buttons.Add(Quit);
        }

        private void QuitGame()
        {
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.Quit;
        }

        private void GoToRules()
        {
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.Instructions;
            this.Back.Selected = true;
            this.Back2.Selected = true;
            this.Start.Selected = false;
            this.Instructions.Selected = false;
            this.Quit.Selected = false;
        }

        private void BackToMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.MainMenu;
            this.Back.Selected = false;
            this.Back2.Selected = false;
            this.Instructions.Selected = true;
        }

        public void ShowMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.MainMenu;
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(this);
            this.Start.Selected = true;
            this.Instructions.Selected = false;
            this.Quit.Selected = false;
        }

        public void HideMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.MainMenu)
            {
                foreach (Button b in this.Buttons)
                {
                    b.Draw(spriteBatch, gameTime);
                }

                this.Splash.Draw(spriteBatch, Vector2.Zero);
            }
            else if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.Instructions)
            {
                this.InstructionsScreen.Draw(spriteBatch, Vector2.Zero);
                this.Back.Draw(spriteBatch, gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.MainMenu)
            {
                foreach (Button b in this.Buttons)
                {
                    b.Update(gameTime);
                }
            }
            else if (TheyDontThinkItBeLikeItIsButItDo.Gamestate == GameState.Instructions)
            {
                this.Back.Update(gameTime);
                this.Back2.Update(gameTime);
            }
        }
    }
}
