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

            if (isButtonJustPressed(Buttons.A, gps, lastGps) && this.agent.Color == Color.Green)
            {
                // vibrate
            }

            if (isButtonJustPressed(Buttons.B, gps, lastGps) && this.agent.Color == Color.Red)
            {
                // vibrate
            }

            if (isButtonJustPressed(Buttons.X, gps, lastGps) && this.agent.Color == Color.Blue)
            {
                // vibrate
            }

            if (isButtonJustPressed(Buttons.Y, gps, lastGps) && this.agent.Color == Color.Yellow)
            {
                // vibrate
            }

            this.lastGps = gps;
        }

        public static bool isButtonJustPressed(Buttons button, GamePadState current, GamePadState last)
        {
            return current.IsButtonDown(button) && last.IsButtonUp(button);
        }
    }
}
