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
        private Agent agent;
        private GamePadState lastGps;
        private PlayerIndex playerIndex;

        public PlayerController(PlayerIndex playerIndex, Agent agent)
        {
            this.agent = agent;
            this.playerIndex = playerIndex;
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
    }
}
