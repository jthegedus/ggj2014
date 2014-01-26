﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GGJ2014.Interfaces;
using GGJ2014.Graphics;
using GGJ2014.Controllers;
using Microsoft.Xna.Framework.Audio;

namespace GGJ2014
{
    public class PregameScreen : IDraw, IUpdate
    {
        private Cue gameMusic;
        public Cue GameMusic { get { return this.gameMusic; } set { this.gameMusic = value; } } 
        private Dictionary<Color, Buttons> ColourButtonMapping;
        private float fadeDelay = 1;
        private const float FadeDuration = 0.5f;
        private float fadeTimer = FadeDuration;
        private float countdown = 3;
        private int displayedCount;
        private int lastDisplayedCount;
        public Button Player1Ready { get; set; }
        public Button Player2Ready { get; set; }
        public Button Player3Ready { get; set; }
        public Button Player4Ready { get; set; }

        public Button[] playerReadyButtons = new Button[WorldManager.NumberOfPlayers];

        List<Button> ButtonList { get; set; }
        
        private const float BaseScale = 0.35f;

        private bool p1Ready = false;
        private bool p2Ready = false;
        private bool p3Ready = false;
        private bool p4Ready = false;

        private bool[] isReady = new bool[WorldManager.NumberOfPlayers];

        private bool first = true;

        public PregameScreen()
        {
        }

        public void Init()
        {
            first = true;
            fadeDelay = 1;
            fadeTimer = FadeDuration;
            countdown = 3;
            p1Ready = false;
            p2Ready = false;
            p3Ready = false;
            p4Ready = false;
            lastDisplayedCount = -1;

            playerReadyButtons = new Button[WorldManager.NumberOfPlayers];

            ColourButtonMapping = new Dictionary<Color, Buttons>();
            ColourButtonMapping.Add(Color.Red, Buttons.B);
            ColourButtonMapping.Add(TheyDontThinkItBeLikeItIsButItDo.Blue, Buttons.X);
            ColourButtonMapping.Add(Color.Yellow, Buttons.Y);
            ColourButtonMapping.Add(Color.Green, Buttons.A);

            Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/notReady");
            PlayerIndex[] playerIndexes = { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
            Vector2[] positions = { new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3),
                                      new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3 * 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3),
                                      new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3 * 2),
                                      new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3 * 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3 * 2) };
            for(int i = 0; i < playerReadyButtons.Length; ++i)
            {
                playerReadyButtons[i] = new Button()
                {
                    BackgroundImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale },
                    Position = positions[i],
                    PlayerIndex = playerIndexes[i],
                    Selected = true,
                    SelectButton = ColourButtonMapping[((PlayerController)TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers[i]).Agent.Color]
                };
            }
            
            /*
            Player1Ready = new Button()
            {
                BackgroundImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale },
                Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3),
                PlayerIndex = PlayerIndex.One,
                Selected = true,
                SelectButton = ColourButtonMapping[(TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers[0] as PlayerController).Agent.Color]
            };

            img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/notReady");
            Player2Ready = new Button()
            {
                BackgroundImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale },
                Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3 * 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3),
                PlayerIndex = PlayerIndex.Two,
                Selected = true,
                SelectButton = ColourButtonMapping[(TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers[1] as PlayerController).Agent.Color]
            };

            img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/notReady");
            Player3Ready = new Button()
            {
                BackgroundImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale },
                Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3 * 2),
                PlayerIndex = PlayerIndex.Three,
                Selected = true,
                SelectButton = ColourButtonMapping[(TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers[2] as PlayerController).Agent.Color]
            };

            img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/notReady");
            Player4Ready = new Button()
            {
                BackgroundImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale },
                Position = new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 3 * 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 3 * 2),
                PlayerIndex = PlayerIndex.Four,
                Selected = true,
                SelectButton = ColourButtonMapping[(TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers[3] as PlayerController).Agent.Color]
            };
             * */

            for (int i = 0; i < WorldManager.NumberOfPlayers; ++i)
            {
                if (playerReadyButtons[i] != null)
                {
                    Action action = null;
                    switch (i)
                    {
                        case 0:
                            action = new Action(ReadyPlayer1);
                            break;
                        case 1:
                            action = new Action(ReadyPlayer2);
                            break;
                        case 2:
                            action = new Action(ReadyPlayer3);
                            break;
                        case 3:
                            action = new Action(ReadyPlayer4);
                            break;
                    }
                    playerReadyButtons[i].OnClick += action;
                    
                }
            }
            /*
             Player1Ready.OnClick += new Action(ReadyPlayer1);
             Player2Ready.OnClick += new Action(ReadyPlayer2);
             Player3Ready.OnClick += new Action(ReadyPlayer3);
             Player4Ready.OnClick += new Action(ReadyPlayer4);
            

            this.ButtonList = new List<Button>();
            this.ButtonList.Add(Player1Ready);
            this.ButtonList.Add(Player2Ready);
            this.ButtonList.Add(Player3Ready);
            this.ButtonList.Add(Player4Ready); */
        }

        private void ReadyPlayer(int player)
        {
            if (!isReady[player])
            {
                Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/ready");
                playerReadyButtons[player].RollOverImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale, zIndex = 0.00001f };
                this.playerReadyButtons[player].Text = new TextElement("Player " + (player+1) + "\n  Ready", this.playerReadyButtons[player].Position, Color.Black, 0) { AnchorPoint = GGJ2014.AnchorPoint.Centre, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, Scale = new Vector2(0.6f) };
                isReady[player] = true;
            }
        }

        private void ReadyPlayer1() { ReadyPlayer(0); }
        private void ReadyPlayer2() { ReadyPlayer(1); }
        private void ReadyPlayer3() { ReadyPlayer(2); }
        private void ReadyPlayer4() { ReadyPlayer(3); }
        /*
        private void ReadyPlayer1()
        {
            if (!p1Ready)
            {
                Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/ready");
                this.Player1Ready.RollOverImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale, zIndex = 0.000001f };
                this.Player1Ready.Text = new TextElement("Player 1\n  Ready", this.Player1Ready.Position, Color.Black, 0) { AnchorPoint = GGJ2014.AnchorPoint.Centre, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, Scale = new Vector2(0.6f) };
                p1Ready = true;
            }
        }

        private void ReadyPlayer2()
        {
            if (!p2Ready)
            {
                Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/ready");
                this.Player2Ready.RollOverImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale, zIndex = 0.000001f };
                this.Player2Ready.Text = new TextElement("Player 2\n  Ready", this.Player2Ready.Position, Color.Black, 0) { AnchorPoint = GGJ2014.AnchorPoint.Centre, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, Scale = new Vector2(0.6f) };
                p2Ready = true;
            }
        }
        private void ReadyPlayer3()
        {
            if (!p3Ready)
            {
                Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/ready");
                this.Player3Ready.RollOverImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale, zIndex = 0.000001f };
                this.Player3Ready.Text = new TextElement("Player 3\n  Ready", this.Player3Ready.Position, Color.Black, 0) { AnchorPoint = GGJ2014.AnchorPoint.Centre, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, Scale = new Vector2(0.6f) };
                p3Ready = true;
            }
        }
        private void ReadyPlayer4()
        {
            if (!p4Ready)
            {
                Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/ready");
                this.Player4Ready.RollOverImage = new Sprite(img, img.Width, img.Height) { AnchorPoint = AnchorPoint.Centre, Zoom = BaseScale * TheyDontThinkItBeLikeItIsButItDo.Scale, zIndex = 0.000001f };
                this.Player4Ready.Text = new TextElement("Player 4\n  Ready", this.Player4Ready.Position, Color.Black, 0) { AnchorPoint = GGJ2014.AnchorPoint.Centre, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, Scale = new Vector2(0.6f) };
                p4Ready = true;
            }
        } */

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
            foreach (Button b in playerReadyButtons)
            {
                b.Draw(spriteBatch, gameTime);
            }
        }

        private bool allPlayersReady()
        {
            for (int i = 0; i < WorldManager.NumberOfPlayers; ++i)
            {
                if (isReady[i] == false)
                    return false;
            }
            return true;
        }
        public void Update(GameTime gameTime)
        {
            // HAX
            if (TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[PlayerIndex.One].IsButtonDown(Buttons.Start))
            {
                this.ReadyPlayer1();
                this.ReadyPlayer2();
                this.ReadyPlayer3();
                this.ReadyPlayer4();
            }

            foreach (Button b in playerReadyButtons)
            {
                b.Update(gameTime);
            }

            if (allPlayersReady())
            {
                float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (fadeDelay >= 0)
                {
                    this.fadeDelay -= elapsedTime;
                }
                else if (this.fadeTimer >= 0)
                {
                    this.fadeTimer -= elapsedTime;
                    foreach (Button b in playerReadyButtons)
                    {
                        float val = MathHelper.Clamp(MathHelper.Lerp(0, 1, this.fadeTimer / FadeDuration), 0, 1);
                        b.RollOverImage.Alpha = val;
                        b.RolloverText.color = new Color(b.RolloverText.color.R, b.RolloverText.color.G, b.RolloverText.color.B, val);
                    }
                }
                else
                {
                    if (first)
                    {
                        first = false;

                        foreach (IController c in TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Controllers)
                        {
                            if (c is PlayerController)
                            {
                                (c as PlayerController).Spawned();
                            }
                        }
                    }

                    this.countdown -= elapsedTime;
                    displayedCount = (int)Math.Ceiling(this.countdown);

                    if (lastDisplayedCount != displayedCount)
                    {
                        lastDisplayedCount = displayedCount;
                        TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(new FadingTextElement(
                            displayedCount.ToString(),
                            new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight / 2),
                            Color.Black,
                            0,
                            1,
                            1,
                            0) { AnchorPoint = AnchorPoint.Centre, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, Scale = new Vector2(2) });
                    }

                    if (this.countdown <= 0)
                    {
                        TheyDontThinkItBeLikeItIsButItDo.Gamestate = GameState.GamePlaying;

                        //Start playing music
                        gameMusic = TheyDontThinkItBeLikeItIsButItDo.AudioManager.LoadCue("backgroundmusic");
                        TheyDontThinkItBeLikeItIsButItDo.AudioManager.PlayCue(ref gameMusic, false);

                        this.HideMenu();
                    }
                }
            }
        }
    }
}
