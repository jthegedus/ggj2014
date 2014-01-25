using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;
using GGJ2014.GameObjects;
using GGJ2014.AI;
using Microsoft.Xna.Framework;

namespace GGJ2014.Controllers
{
    class AIController : IController
    {
        private Agent agent;
        private Path path;

        private Agent Target { get; set; }

        public AIController(Agent agent)
        {
            this.agent = agent;
        }

        public void HandleInput()
        {
            if (path == null)
            {
                // TESTING PATHFINDING
                path = Path.pathfind(new Vector2(14, 15), new Vector2(46, 11), TheyDontThinkItBeLikeItIsButItDo.WorldManager.Level);
                TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(path);
            }
            // Do nothing
        }
    }
}
