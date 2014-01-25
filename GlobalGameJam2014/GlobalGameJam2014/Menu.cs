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
    public class Menu : IDraw
    {
        private int selected = 0;
        private const int MAX_CHOICES = 3;
        private TextElement[] choices = new TextElement[MAX_CHOICES];

        private float lastUpdate = 0f;
        public const float threshold = 0.1f;

        public Menu()
        {
            choices[0] = new TextElement("PLAY!",
                new Vector2(20, 20), Color.Red, 1f);
            choices[1] = new TextElement("Instructions",
                new Vector2(20, 40), Color.Yellow, 1f);
            choices[2] = new TextElement("Exit",
                new Vector2(20, 60), Color.Yellow, 1f);
        }

        public void ShowMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(this);
        }

        public void HideMenu()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
        }

        public void Update(GameTime gametime)
        {
            if ((float)gametime.TotalGameTime.TotalSeconds - lastUpdate > threshold)
            {
                GamePadState gps = GamePad.GetState(PlayerIndex.One);
                Vector2 lThumb = gps.ThumbSticks.Left;
                if (lThumb != Vector2.Zero)
                {
                    if (Math.Acos(Vector2.Dot(lThumb, Vector2.UnitY)) < MathHelper.PiOver4 && selected > 0)
                    {
                        ChangeSelection(selected, --selected);
                    }
                    else if (Math.Acos(Vector2.Dot(lThumb, -Vector2.UnitY)) < MathHelper.PiOver4 && selected < MAX_CHOICES - 1)
                    {
                        ChangeSelection(selected, ++selected);
                    }

                    lastUpdate = (float)gametime.TotalGameTime.TotalSeconds;
                }

                if (gps.IsButtonDown(Buttons.A))
                {
                    this.HideMenu();

                    if (selected == 0)
                    {
                        TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.GAMEPLAYING;
                        TheyDontThinkItBeLikeItIsButItDo.WorldManager.InitGame();
                    }
                    else if (selected == 1)
                        TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.INSTRUCTIONS;
                    else if (selected == 2)
                        TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.GAMEENDED;
                }
            }
        }

        private void ChangeSelection(int from, int to)
        {
            choices[from].color = Color.Yellow;
            choices[to].color = Color.Red;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (TextElement text in this.choices)
            {
                text.Draw(spriteBatch, gameTime);
            }
        }
    }
}
