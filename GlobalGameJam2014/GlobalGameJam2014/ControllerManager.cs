using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using GGJ2014.Controllers;
using Microsoft.Xna.Framework;

namespace GGJ2014
{
    public class ControllerManager
    {
        public List<IController> Controllers { get; private set; }

        public ControllerManager()
        {
            this.Controllers = new List<IController>();
        }

        public void AddController(IController controller)
        {
            this.Controllers.Add(controller);
        }

        public void Update()
        {
            foreach (IController controller in this.Controllers)
            {
                controller.HandleInput();
            }
        }

        public PlayerController GetPlayerController(int player)
        {
            PlayerIndex playerIndex = PlayerIndex.One;
            switch (player)
            {
                case 0: playerIndex = PlayerIndex.One; break;
                case 1: playerIndex = PlayerIndex.Two; break;
                case 2: playerIndex = PlayerIndex.Three; break;
                case 3: playerIndex = PlayerIndex.Four; break;
            }
            foreach (IController c in Controllers)
            {
                if (c is PlayerController && ((PlayerController)c).PlayerIndex == playerIndex)
                    return (PlayerController)c;
            }
            return null;
        }


        public void ClearLists()
        {
            this.Controllers.Clear();
        }

        public void ResetScores()
        {
            foreach (IController c in this.Controllers)
            {
                if (c is PlayerController)
                {
                    (c as PlayerController).Score = 0;
                }
            }
        }
    }
}
