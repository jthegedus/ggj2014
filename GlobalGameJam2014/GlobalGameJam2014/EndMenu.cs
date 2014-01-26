using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GGJ2014.Interfaces;
using GGJ2014.Graphics;
using GGJ2014.Controllers;

namespace GGJ2014
{
    public class EndMenu : IDraw, IUpdate
    {
        public List<Button> Buttons { get; set; }
        public TextElement Winner { get; set; }
        public Button PlayAgain { get; set; }
        public Button QuitToMenu { get; set; }
        public Sprite Background { get; set; }
        public Sprite GameOver { get; set; }

        public EndMenu()
        {
            this.Buttons = new List<Button>();

            Vector2 position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 5 * 3);
            this.PlayAgain = new Button()
            {
                Position = position,
                Text = new TextElement("Play Again", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                RolloverText = new TextElement("Play Again", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickText = new TextElement("Play Again", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickDuration = 0.5f,
            };

            position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 5 * 4);
            this.QuitToMenu = new Button()
            {
                Position = position,
                Text = new TextElement("Quit to Menu", position, Color.Black, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                RolloverText = new TextElement("Quit to Menu", position, Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickText = new TextElement("Quit to Menu", position, Color.DarkRed, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont },
                ClickDuration = 0.5f,
            };

            Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/gameOver");
            this.GameOver = new Sprite(img, img.Width, img.Height, 0)
            {
                AnchorPoint = GGJ2014.AnchorPoint.Centre,
                Zoom = img.Width / (TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3)
            };

            PlayAgain.Selected = true;

            PlayAgain.OnClick += new Action(this.HideMenu);
            PlayAgain.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.WorldManager.InitGame);
            PlayAgain.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.PregameScreen.Init);
            PlayAgain.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.PregameScreen.ShowMenu);
            QuitToMenu.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.Menu.ShowMenu);
            QuitToMenu.OnClick += new Action(this.HideMenu);
            QuitToMenu.OnClick += new Action(TheyDontThinkItBeLikeItIsButItDo.GameUI.HideUI);

            PlayAgain.ButtonAbove = this.QuitToMenu;
            PlayAgain.ButtonBelow = this.QuitToMenu;

            QuitToMenu.ButtonAbove = this.PlayAgain;
            QuitToMenu.ButtonBelow = this.PlayAgain;

            this.Buttons.Add(PlayAgain);
            this.Buttons.Add(QuitToMenu);

            
            this.Background = new Sprite(TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/agent"), (int)TheyDontThinkItBeLikeItIsButItDo.ScreenWidth, (int)TheyDontThinkItBeLikeItIsButItDo.ScreenHeight, 0);
        }

        public void ShowMenu()
        {
            //play cheer sound
            Microsoft.Xna.Framework.Audio.Cue cheer = TheyDontThinkItBeLikeItIsButItDo.AudioManager.LoadCue("smallapplause");
            TheyDontThinkItBeLikeItIsButItDo.AudioManager.PlayCue(ref cheer, false);

            String winner = "";
            int score = -1;
            foreach (IController controller in TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers)
            {
                if (controller is PlayerController)
                {
                    PlayerController c = controller as PlayerController;
                    if (c.Score > score)
                    {
                        score = c.Score;
                        switch (c.PlayerIndex)
                        {
                            case PlayerIndex.One:
                                winner = "Player 1";
                                break;
                            case PlayerIndex.Two:
                                winner = "Player 2";
                                break;
                            case PlayerIndex.Three:
                                winner = "Player 3";
                                break;
                            case PlayerIndex.Four:
                                winner = "Player 4";
                                break;
                        }
                    }
                    else if (score == c.Score)
                    {
                        switch (c.PlayerIndex)
                        {
                            case PlayerIndex.One:
                                winner += " and 1";
                                break;
                            case PlayerIndex.Two:
                                winner += " and 2";
                                break;
                            case PlayerIndex.Three:
                                winner += " and 3";
                                break;
                            case PlayerIndex.Four:
                                winner += " and 4";
                                break;
                        }
                    }
                }

                this.PlayAgain.Selected = true;
                this.QuitToMenu.Selected = false;
            }

            if (winner.Length > 8)
            {
                winner += " win!";
            }
            else
            {
                winner += " wins!";
            }
            this.Winner = new TextElement(winner, new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 5 * 2), Color.Red, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont };
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

            // this.Background.Draw(spriteBatch, Vector2.Zero);
            this.Winner.Draw(spriteBatch, gameTime);
            this.GameOver.Draw(spriteBatch, new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 5));
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
