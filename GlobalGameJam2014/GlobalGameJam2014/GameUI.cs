using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GGJ2014.Controllers;

namespace GGJ2014
{
    public class GameUI : IDraw
    {
        public TextElement Player1Score { get; set; }
        public TextElement Player2Score { get; set; }
        public TextElement Player3Score { get; set; }
        public TextElement Player4Score { get; set; }
        public TextElement Player1Objective { get; set; }
        public TextElement Player2Objective { get; set; }
        public TextElement Player3Objective { get; set; }
        public TextElement Player4Objective { get; set; }
        public TextElement GameTimer { get; set; }

        public GameUI()
        {
        }

        public void Init()
        {
            this.GameTimer = new TextElement("60", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 20), Color.Black, 0) { AnchorPoint = AnchorPoint.Bottom, Font = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<SpriteFont>("SpriteFonts/Arial64Bold") };
            Color textColor = Color.Black;
            this.Player1Score = new TextElement("Player 1: 0000", new Vector2(20, 20), textColor, 0) { AnchorPoint = AnchorPoint.TopLeft };
            this.Player2Score = new TextElement("Player 2: 0000", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 20, 20), textColor, 0) { AnchorPoint = AnchorPoint.TopRight };
            this.Player3Score = new TextElement("Player 3: 0000", new Vector2(20, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50), textColor, 0) { AnchorPoint = AnchorPoint.BottomLeft };
            this.Player4Score = new TextElement("Player 4: 0000", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 20, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50), textColor, 0) { AnchorPoint = AnchorPoint.BottomRight };
            this.Player1Objective = new TextElement("", new Vector2(20, 50), textColor, 0) { AnchorPoint = AnchorPoint.TopLeft };
            this.Player2Objective = new TextElement("", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 20, 50), textColor, 0) { AnchorPoint = AnchorPoint.TopRight };
            this.Player3Objective = new TextElement("", new Vector2(20, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 20), textColor, 0) { AnchorPoint = AnchorPoint.BottomLeft };
            this.Player4Objective = new TextElement("", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 20, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 20), textColor, 0) { AnchorPoint = AnchorPoint.BottomRight };
        }

        public void ShowUI()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(this);
        }

        public void HideUI()
        {
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.GameTimer.Draw(spriteBatch, gameTime);
            this.Player1Score.Draw(spriteBatch, gameTime);
            this.Player2Score.Draw(spriteBatch, gameTime);
            this.Player3Score.Draw(spriteBatch, gameTime);
            this.Player4Score.Draw(spriteBatch, gameTime);
            this.Player1Objective.Draw(spriteBatch, gameTime);
            this.Player2Objective.Draw(spriteBatch, gameTime);
            this.Player3Objective.Draw(spriteBatch, gameTime);
            this.Player4Objective.Draw(spriteBatch, gameTime);
        }
    }
}
