using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework;
using GGJ2014.Graphics;
using Microsoft.Xna.Framework.Input;
using GGJ2014.GameObjects;
using GGJ2014.Controllers;

namespace GGJ2014
{
    public class Button : IUpdate, IDraw
    {
        private Sprite backgroundImage;
        private Vector2 position;
        public Vector2 Position 
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
                if (this.Text != null)
                {
                    this.Text.pos = value;
                }
                if (this.RolloverText != null)
                {
                    this.RolloverText.pos = value;
                }
                if (this.ClickText != null)
                {
                    this.ClickText.pos = value;
                }
            }
        }
        public Sprite BackgroundImage
        {
            get
            {
                return this.backgroundImage;
            }
            set
            {
                this.backgroundImage = value;
                if (this.RollOverImage == null)
                {
                    this.RollOverImage = value;
                }
                if (this.ClickImage == null)
                {
                    this.ClickImage = value;
                }
            }
        }
        public Sprite RollOverImage { get; set; }
        public Sprite ClickImage { get; set; }
        private TextElement text;
        public TextElement Text 
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                if (this.RolloverText == null)
                {
                    this.RolloverText = value;
                }
                if (this.ClickText == null)
                {
                    this.ClickText = value;
                }
            }
        }
        public TextElement RolloverText { get; set; }
        public TextElement ClickText { get; set; }
        public float ClickDuration { get; set; }
        public Button ButtonBelow { get; set; }
        public Button ButtonAbove { get; set; }
        public Button ButtonLeft { get; set; }
        public Button ButtonRight { get; set; }
        public bool Selected { get; set; }
        public event Action OnClick;
        private float clickTimer;
        public static bool ButtonChangedThisUpdate { get; set; }
        public PlayerIndex PlayerIndex { get; set; }
        public Buttons SelectButton { get; set; }

        public Button()
        {
            this.PlayerIndex = PlayerIndex.One;
            this.SelectButton = Buttons.A;
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            TheyDontThinkItBeLikeItIsButItDo.ControllerManager.Update();

            GamePadState gps = TheyDontThinkItBeLikeItIsButItDo.CurrentGPS[this.PlayerIndex];
            GamePadState lastGps = TheyDontThinkItBeLikeItIsButItDo.LastGPS[this.PlayerIndex];

            if (clickTimer != 0)
            {
                this.clickTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (this.clickTimer <= 0)
                {
                    this.clickTimer = 0;
                    if (OnClick != null)
                        OnClick();
                }
            }
            else if (this.Selected)
            {
                if (PlayerController.IsButtonJustPressed(this.SelectButton, gps, lastGps))
                {
                    if (this.ClickDuration > 0)
                    {
                        this.clickTimer = this.ClickDuration;
                    }
                    else
                    {
                        if (OnClick != null)
                            OnClick();
                    }
                }
                else if (!ButtonChangedThisUpdate)
                {
                    if (PlayerController.IsButtonJustPressed(Buttons.DPadDown, gps, lastGps) && this.ButtonBelow != null)
                    {
                        this.Selected = false;
                        this.ButtonBelow.Selected = true;
                        Button.ButtonChangedThisUpdate = true;
                    }
                    else if (PlayerController.IsButtonJustPressed(Buttons.DPadUp, gps, lastGps) && this.ButtonAbove != null)
                    {
                        this.Selected = false;
                        this.ButtonAbove.Selected = true;
                        Button.ButtonChangedThisUpdate = true;
                    }
                    else if (PlayerController.IsButtonJustPressed(Buttons.DPadLeft, gps, lastGps) && this.ButtonLeft != null)
                    {
                        this.Selected = false;
                        this.ButtonLeft.Selected = true;
                        Button.ButtonChangedThisUpdate = true;
                    }
                    else if (PlayerController.IsButtonJustPressed(Buttons.DPadRight, gps, lastGps) && this.ButtonRight != null)
                    {
                        this.Selected = false;
                        this.ButtonRight.Selected = true;
                        Button.ButtonChangedThisUpdate = true;
                    }
                }
            }
        }

        public void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (clickTimer == 0)
            {
                if (!this.Selected)
                {
                    if (this.BackgroundImage != null)
                    {
                        this.BackgroundImage.Draw(spriteBatch, this.Position);
                    }
                    if (this.Text != null)
                    {
                        this.Text.Draw(spriteBatch, gameTime);
                    }
                }
                else
                {
                    if (this.RollOverImage != null)
                    {
                        this.RollOverImage.Draw(spriteBatch, this.Position);
                    }
                    if (this.RolloverText != null)
                    {
                        this.RolloverText.Draw(spriteBatch, gameTime);
                    }
                }
            }
            else if (clickTimer > ClickDuration / 2)
            {
                if (this.ClickImage != null)
                {
                    this.ClickImage.Draw(spriteBatch, this.Position);
                }
                if (this.ClickText != null)
                {
                    this.ClickText.Draw(spriteBatch, gameTime);
                }
            }
            else
            {
                if (this.RollOverImage != null)
                {
                    this.RollOverImage.Draw(spriteBatch, this.Position);
                }
                if (this.RolloverText != null)
                {
                    this.RolloverText.Draw(spriteBatch, gameTime);
                }
            }
        }
    }
}
