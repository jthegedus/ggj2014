using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GGJ2014.Controllers;
using GGJ2014.Graphics;

namespace GGJ2014
{
    public class GameUI : IDraw
    {
        public Texture2D Baseball { get; set; }
        public Texture2D Glove { get; set; }
        public Sprite P1ObjectiveIcon { get; set; }
        public Sprite P1TargetIcon { get; set; }
        public Sprite P2ObjectiveIcon { get; set; }
        public Sprite P2TargetIcon { get; set; }
        public Sprite P3ObjectiveIcon { get; set; }
        public Sprite P3TargetIcon { get; set; }
        public Sprite P4ObjectiveIcon { get; set; }
        public Sprite P4TargetIcon { get; set; }
        private List<TextElement> texts;
        public TextElement Player1Score { get; set; }
        public TextElement Player2Score { get; set; }
        public TextElement Player3Score { get; set; }
        public TextElement Player4Score { get; set; }
        public TextElement Player1Objective { get; set; }
        public TextElement Player2Objective { get; set; }
        public TextElement Player3Objective { get; set; }
        public TextElement Player4Objective { get; set; }
        public TextElement GameTimer { get; set; }
        private float targetIconScale = 0.17f;
        private float objectiveIconScale = 0.12f;

        public GameUI()
        {
        }

        public void Init()
        {
            this.GameTimer = new TextElement(TheyDontThinkItBeLikeItIsButItDo.WorldManager.GameTimer.ToString(), new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth / 2, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight), Color.Black, 0) { AnchorPoint = AnchorPoint.Bottom, Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont };
            Color textColor = Color.Black;
            this.Player1Score = new TextElement("P1: 000", new Vector2(90, 0), textColor, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopLeft, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.3f };
            this.Player2Score = new TextElement("P2: 000", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 90, 0), textColor, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.TopRight, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.3f };
            this.Player3Score = new TextElement("P3: 000", new Vector2(90, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50), textColor, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomLeft, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.3f };
            this.Player4Score = new TextElement("P4: 000", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 90, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 50), textColor, 0) { Font = TheyDontThinkItBeLikeItIsButItDo.LargeFont, AnchorPoint = AnchorPoint.BottomRight, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.3f };
            this.Player1Objective = new TextElement("", new Vector2(90, 45), textColor, 0) { AnchorPoint = AnchorPoint.TopLeft, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.4f};
            this.Player2Objective = new TextElement("", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 95, 45), textColor, 0) { AnchorPoint = AnchorPoint.TopRight, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.4f };
            this.Player3Objective = new TextElement("", new Vector2(90, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 35), textColor, 0) { AnchorPoint = AnchorPoint.BottomLeft, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.4f };
            this.Player4Objective = new TextElement("", new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 95, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 35), textColor, 0) { AnchorPoint = AnchorPoint.BottomRight, Scale = new Vector2(TheyDontThinkItBeLikeItIsButItDo.Scale) * 0.4f };

            this.texts = new List<TextElement>();
            this.texts.Add(Player1Score);
            this.texts.Add(Player2Score);
            this.texts.Add(Player3Score);
            this.texts.Add(Player4Score);
            this.texts.Add(Player1Objective);
            this.texts.Add(Player2Objective);
            this.texts.Add(Player3Objective);
            this.texts.Add(Player4Objective);

            Texture2D img = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/circle");
            this.P1TargetIcon = new Sprite(img, img.Width, img.Height, 0.000001f) { AnchorPoint = AnchorPoint.Centre, Zoom = targetIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
            this.P2TargetIcon = new Sprite(img, img.Width, img.Height, 0.000001f) { AnchorPoint = AnchorPoint.Centre, Zoom = targetIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
            this.P3TargetIcon = new Sprite(img, img.Width, img.Height, 0.000001f) { AnchorPoint = AnchorPoint.Centre, Zoom = targetIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
            this.P4TargetIcon = new Sprite(img, img.Width, img.Height, 0.000001f) { AnchorPoint = AnchorPoint.Centre, Zoom = targetIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };

            Baseball = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/baseballLarge");
            Glove = TheyDontThinkItBeLikeItIsButItDo.ContentManager.Load<Texture2D>("Sprites/glove");
            this.P1ObjectiveIcon = new Sprite(Baseball, img.Width, img.Height, 0) { AnchorPoint = AnchorPoint.Centre, Zoom = objectiveIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
            this.P2ObjectiveIcon = new Sprite(Baseball, img.Width, img.Height, 0) { AnchorPoint = AnchorPoint.Centre, Zoom = objectiveIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
            this.P3ObjectiveIcon = new Sprite(Baseball, img.Width, img.Height, 0) { AnchorPoint = AnchorPoint.Centre, Zoom = objectiveIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
            this.P4ObjectiveIcon = new Sprite(Baseball, img.Width, img.Height, 0) { AnchorPoint = AnchorPoint.Centre, Zoom = objectiveIconScale * TheyDontThinkItBeLikeItIsButItDo.Scale };
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

            this.P1TargetIcon.Draw(spriteBatch, new Vector2(50, 40));
            this.P2TargetIcon.Draw(spriteBatch, new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, 40));
            this.P3TargetIcon.Draw(spriteBatch, new Vector2(50,  TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 65));
            this.P4TargetIcon.Draw(spriteBatch, new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 65));

            this.P1ObjectiveIcon.Draw(spriteBatch, new Vector2(50, 40));
            this.P2ObjectiveIcon.Draw(spriteBatch, new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, 40));
            this.P3ObjectiveIcon.Draw(spriteBatch, new Vector2(50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 65));
            this.P4ObjectiveIcon.Draw(spriteBatch, new Vector2(TheyDontThinkItBeLikeItIsButItDo.ScreenWidth - 50, TheyDontThinkItBeLikeItIsButItDo.ScreenHeight - 65));  
        }
    }
}
