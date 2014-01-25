using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework.Input;

namespace GGJ2014.Controllers
{
    public class TimedVibration : IUpdate
    {
        public PlayerIndex PlayerIndex { get; set; }
        public float Duration { get; set; }
        private float timer;
        public float Strength { get; set; }

        public TimedVibration(PlayerIndex playerIndex, float strength, float duration)
        {
            this.PlayerIndex = playerIndex;
            this.Duration = duration;
            this.Strength = strength;
            this.timer = duration;
            GamePad.SetVibration(this.PlayerIndex, strength, strength);
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.timer -= elapsedTime;
            if (timer <= 0)
            {
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(this);
                GamePad.SetVibration(this.PlayerIndex, 0, 0);
            }
        }
    }
}
