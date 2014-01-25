using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GGJ2014.GameObjects;
using GGJ2014.Components;
using GGJ2014.Interfaces;

namespace GGJ2014.Controllers
{
    public class PlayerController : IController
    {
        public static readonly List<Color> Colors = new List<Color>() { Color.Red, Color.Blue, Color.Yellow, Color.Green };
        private Color previousTarget;
        private Objectives previousObjective;
        public Color Target { get; set; }
        public Objectives Objective { get; set; }
        private int score;
        public int Score
        {
            get
            {
                return this.score;
            }
            
            set
            {
                this.score = value;
                switch (this.playerIndex)
                {
                    case PlayerIndex.One:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player1Score.text = "Player 1: " + String.Format("{0:d4}", this.score);
                        break;
                    case PlayerIndex.Two:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player1Score.text = "Player 2: " + this.score.ToString();
                        break;
                    case PlayerIndex.Three:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player1Score.text = "Player 3: " + this.score.ToString();
                        break;
                    case PlayerIndex.Four:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player1Score.text = "Player 4: " + this.score.ToString();
                        break;
                }
            }
        }
        private Agent agent;
        private GamePadState lastGps;
        private PlayerIndex playerIndex;

        public PlayerController(PlayerIndex playerIndex, Agent agent)
        {
            this.agent = agent;
            this.agent.Controller = this;
            this.playerIndex = playerIndex;
            this.previousTarget = this.agent.Color;
            this.Target = this.agent.Color;
            this.GenerateObjective();
        }

        public void HandleInput()
        {
            GamePadState gps = GamePad.GetState(this.playerIndex);

            this.agent.DesiredMovementDirection = gps.ThumbSticks.Left;
            this.agent.ShootDirection = gps.ThumbSticks.Right;

            if (gps.IsButtonDown(Buttons.A) && this.agent.Color == Color.Green)
            {
                GamePad.SetVibration(this.playerIndex, 1, 1);
            }
            else if (gps.IsButtonDown(Buttons.B) && this.agent.Color == Color.Red)
            {
                GamePad.SetVibration(this.playerIndex, 1, 1);
            }
            else if (gps.IsButtonDown(Buttons.X) && this.agent.Color == Color.Blue)
            {
                GamePad.SetVibration(this.playerIndex, 1, 1);
            }
            else if (gps.IsButtonDown(Buttons.Y) && this.agent.Color == Color.Yellow)
            {
                GamePad.SetVibration(this.playerIndex, 1, 1);
            }
            else
            {
                GamePad.SetVibration(this.playerIndex, 0, 0);
            }

            this.lastGps = gps;
        }

        public static bool isButtonJustPressed(Buttons button, GamePadState current, GamePadState last)
        {
            return current.IsButtonDown(button) && last.IsButtonUp(button);
        }


        public void DamagedPlayer(Agent victim)
        {
            if (victim.Color == this.Target && this.Objective == Objectives.Kill)
            {
                this.Score += Scores.PlayerDamagedReward;
            }
            else
            {
                this.Score += Scores.PlayerDamagedPenalty;
            }
        }

        public void KilledPlayer(Agent victim)
        {
            if (victim.Color == this.Target && this.Objective == Objectives.Kill)
            {
                this.Score += Scores.PlayerKilledReward;
                this.GenerateObjective();
            }
            else
            {
                this.Score += Scores.PlayerKilledPenalty;
                this.GenerateObjective();
            }
        }

        public void BumpedPlayer(Agent victim)
        {
            if (victim.Color == this.Target && this.Objective == Objectives.HighFive)
            {
                this.Score += Scores.BumpReward;
                this.GenerateObjective();
            }
        }

        public void CollectedCollectible()
        {
            this.Score += Scores.CollectibleReward;
        }

        public void GenerateObjective()
        {
            this.previousObjective = this.Objective;
            this.previousTarget = this.Target;

            do
            {
                {
                    do
                    {
                        this.Target = PlayerController.Colors[TheyDontThinkItBeLikeItIsButItDo.Rand.Next(4)];
                    } while (Target == this.agent.Color) ;

                    this.Objective = (Objectives)TheyDontThinkItBeLikeItIsButItDo.Rand.Next(2);
                }
            } while (this.Target == this.previousTarget && this.previousObjective == this.Objective);

            switch (this.playerIndex)
            {
                case PlayerIndex.One:
                    TheyDontThinkItBeLikeItIsButItDo.GameUI.Player1Objective.text = this.GetObjectiveString();
                    break;
                case PlayerIndex.Two:
                    TheyDontThinkItBeLikeItIsButItDo.GameUI.Player2Objective.text = this.GetObjectiveString();
                    break;
                case PlayerIndex.Three:
                    TheyDontThinkItBeLikeItIsButItDo.GameUI.Player3Objective.text = this.GetObjectiveString();
                    break;
                case PlayerIndex.Four:
                    TheyDontThinkItBeLikeItIsButItDo.GameUI.Player4Objective.text = this.GetObjectiveString();
                    break;
            }
        }

        private String GetObjectiveString()
        {
            StringBuilder objective = new StringBuilder();
            
            switch (this.Objective)
            {
                case Objectives.HighFive:
                    objective.Append("Tag ");
                    break;
                case Objectives.Kill:
                    objective.Append("Kill ");
                    break;
            }

            if (this.Target == Color.Red)
            {
                objective.Append("Red");
            }
            else if (this.Target == Color.Yellow)
            {
                objective.Append("Yellow");
            }
            else if (this.Target == Color.Green)
            {
                objective.Append("Green");
            }
            else if (this.Target == Color.Blue)
            {
                objective.Append("Blue");
            }

            return objective.ToString();
        }
    }
}
