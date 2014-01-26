using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GGJ2014.GameObjects;
using GGJ2014.Components;
using GGJ2014.Interfaces;
using GGJ2014.Graphics;

namespace GGJ2014.Controllers
{
    public class PlayerController : IUpdate, IController
    {
        public static readonly List<String> BadThings = new List<String> { "Nope.", "Wrong.", "Bad.", "Derp.", "No.", "Uh-uh.", "Nup." };
        public static readonly List<Color> Colors = new List<Color>() { Color.Red, Color.Blue, Color.Yellow, Color.Green };
        private static readonly List<Vector2> ScoreDirections = new List<Vector2> { -Vector2.UnitY, Vector2.Normalize(new Vector2(1, -1)), Vector2.UnitX, Vector2.Normalize(new Vector2(1, 1)), Vector2.UnitY, Vector2.Normalize(new Vector2(-1, 1)), -Vector2.UnitX, Vector2.Normalize(new Vector2(-1, -1)) };
        private int scoreTurn = 0;
        private Color previousTarget;
        private Objectives previousObjective;
        public Color Target { get; set; }
        public Objectives Objective { get; set; }
        private int score;
        private bool first = true;

        public const float PenaltyDuration = 3f;
        private float penaltyTimer = PenaltyDuration;
        private bool penalty;
        public bool Penalty { 
            get
            {
                return penalty;
            }
            set
            {
                if (value == true && this.penalty == false)
                {
                    this.Agent.DesiredMovementDirection = Vector2.Zero;
                    this.Agent.ShootDirection = Vector2.Zero;
                    this.Agent.Firing = false;
                    FadingTextElement fte = new FadingTextElement("Penalty!", this.Agent, Color.Black, 0, PenaltyDuration, 1, 1);
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(fte);
                }
                this.Agent.Visible = value;
                this.penalty = value;
            }
                
        }

        public int Score
        {
            get
            {
                return this.score;
            }

            set
            {
                int diff = value - score;
                this.score = value;
                switch (this.PlayerIndex)
                {
                    case PlayerIndex.One:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player1Score.text = "Player 1: " + String.Format("{0:d4}", this.score);
                        break;
                    case PlayerIndex.Two:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player2Score.text = "Player 2: " + String.Format("{0:d4}", this.score);
                        break;
                    case PlayerIndex.Three:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player3Score.text = "Player 3: " + String.Format("{0:d4}", this.score);
                        break;
                    case PlayerIndex.Four:
                        TheyDontThinkItBeLikeItIsButItDo.GameUI.Player4Score.text = "Player 4: " + String.Format("{0:d4}", this.score);
                        break;
                }

                // display score
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(
                    new FadingTextElement(
                        true ? diff.ToString() : PlayerController.BadThings[TheyDontThinkItBeLikeItIsButItDo.Rand.Next(PlayerController.BadThings.Count)],
                        this.Agent.TransformComponent.Position + ScoreDirections[scoreTurn] * 30,
                        Color.Black,
                        0,
                        0.5f,
                        1,
                        0.1f) { AnchorPoint = AnchorPoint.Centre });

                ++this.scoreTurn;
                if (this.scoreTurn >= 8)
                    this.scoreTurn = 0;
            }
        }
        public Agent Agent { get; private set; }
        private GamePadState lastGps;
        public PlayerIndex PlayerIndex { get; set; }

        public PlayerController(PlayerIndex playerIndex, Agent agent)
        {
            this.Agent = agent;
            this.Agent.Controller = this;
            this.PlayerIndex = playerIndex;
            this.previousTarget = this.Agent.Color;
            this.Target = this.Agent.Color;
            this.GenerateObjective();
        }

        public void HandleInput()
        {
            GamePadState gps = GamePad.GetState(this.PlayerIndex);

            if (!Penalty)
            {
                this.Agent.DesiredMovementDirection = gps.ThumbSticks.Left;
                this.Agent.ShootDirection = gps.ThumbSticks.Right;

                // Color Identification
                if (IsButtonJustPressed(Buttons.A, gps, lastGps) && this.Agent.Color == Color.Green)
                {
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(new TimedVibration(this.PlayerIndex, 1f, 0.25f));
                }
                else if (IsButtonJustPressed(Buttons.B, gps, lastGps) && this.Agent.Color == Color.Red)
                {
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(new TimedVibration(this.PlayerIndex, 1f, 0.25f));
                }
                else if (IsButtonJustPressed(Buttons.X, gps, lastGps) && this.Agent.Color == Color.Blue)
                {
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(new TimedVibration(this.PlayerIndex, 1f, 0.25f));
                }
                else if (IsButtonJustPressed(Buttons.Y, gps, lastGps) && this.Agent.Color == Color.Yellow)
                {
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(new TimedVibration(this.PlayerIndex, 1f, 0.25f));
                }

                // Dash
                if (IsButtonJustPressed(Buttons.LeftTrigger, gps, lastGps))
                {
                    // Dash
                    Agent.Dash();
                }
            }
            this.lastGps = gps;
        }

        public void Update(GameTime gameTime)
        {
            if (Penalty)
            {
                penaltyTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (penaltyTimer <= 0)
                {
                    penaltyTimer = PenaltyDuration;
                    Penalty = false;
                }
            }
        }

        public static bool IsButtonJustPressed(Buttons button, GamePadState current, GamePadState last)
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
                if (victim.Controller != null && victim.Controller is AIController)
                {
                    ((AIController)victim.Controller).ConsiderNewTarget(this);
                }
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
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.SetPenaltyChase(this);
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

        public void Died()
        {
            // Do nothing
        }

        public void Spawned()
        {
            FadingTextElement fte = new FadingTextElement("Player " + PlayerIndex, Agent, Color.Black, 0, 4f, 1f, 0f);
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(fte);
        }

        public void GenerateObjective()
        {
            if (!first)
            {
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(new TimedVibration(this.PlayerIndex, 0.5f, 0.25f));
            }
            else
            {
                first = false;
            }

            // set the previous objective to the current objective
            this.previousObjective = this.Objective;
            this.previousTarget = this.Target;

            // generate a new objective
            // while the objective hasn't changed
            while (this.Target == this.previousTarget && this.previousObjective == this.Objective)
            {
                do
                {
                    this.Target = PlayerController.Colors[TheyDontThinkItBeLikeItIsButItDo.Rand.Next(WorldManager.NumberOfPlayers)];
                } while (Target == this.Agent.Color);

                this.Objective = (Objectives)TheyDontThinkItBeLikeItIsButItDo.Rand.Next(2);
            }

            switch (this.PlayerIndex)
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
