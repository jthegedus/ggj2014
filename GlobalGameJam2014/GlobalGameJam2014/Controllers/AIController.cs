using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using GGJ2014.GameObjects;

namespace GGJ2014.Controllers
{
    class AIController : IController
    {
        private Agent agent;

        private Agent Target { get; set; }

        public AIController(Agent agent)
        {
            this.agent = agent;
        }

        public void HandleInput()
        {
            throw new NotImplementedException();
        }
    }
}
