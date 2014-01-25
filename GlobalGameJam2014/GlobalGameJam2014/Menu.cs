using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GGJ2014.Interfaces;

namespace GGJ2014
{
    public class Menu : IDraw, IUpdate
    {
        public List<Button> Buttons { get; set; }
        public Button Start { get; set; }
        public Button Instructions { get; set; }
        public Button Quit { get; set; }

        public Menu()
        {
            this.Buttons = new List<Button>();

            Vector2 position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 4);
            this.Start = new Button()
            {
                Position = position,
                Text = new TextElement("Start", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                RolloverText = new TextElement("Start", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickText = new TextElement("Start", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickDuration = 0.5f,
            };

            position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 4 * 2);
            this.Instructions = new Button()
            {
                Position = position,
                Text = new TextElement("Instructions", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                RolloverText = new TextElement("Instructions", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickText = new TextElement("Instructions", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickDuration = 0.5f,
            };

            position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 4 * 3);
            this.Quit = new Button()
            {
                Position = position,
                Text = new TextElement("Quit", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                RolloverText = new TextElement("Quit", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickText = new TextElement("Quit", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickDuration = 0.5f
            };

            Start.Selected = true;

            Start.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.WorldManager.InitGame);
            Start.OnClick += new Action(this.HideMenu);
            // Instructions.OnClick += new Action();
            Quit.OnClick += new Action(this.QuitGame);

            Start.ButtonAbove = this.Quit;
            Start.ButtonBelow = this.Instructions;

            Instructions.ButtonAbove = this.Start;
            Instructions.ButtonBelow = this.Quit;

            Quit.ButtonAbove = this.Instructions;
            Quit.ButtonBelow = this.Start;

            this.Buttons.Add(Start);
            this.Buttons.Add(Instructions);
            this.Buttons.Add(Quit);
        }

        private void QuitGame()
        {
            TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.Quit;
        }

        public void ShowMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(this);
        }

        public void HideMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Button b in this.Buttons)
            {
                b.Draw(spriteBatch, gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Button b in this.Buttons)
            {
                b.Update(gameTime);
            }
        }
    }
}
