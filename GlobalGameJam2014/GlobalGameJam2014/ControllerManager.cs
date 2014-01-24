using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;

namespace GGJ2014
{
    public class ControllerManager
    {
        private List<IController> controllers;

        public ControllerManager()
        {
            this.controllers = new List<IController>();
        }

        public void AddController(IController controller)
        {
            this.controllers.Add(controller);
        }

        public void Update()
        {
            foreach (IController controller in this.controllers)
            {
                controller.HandleInput();
            }
        }
    }
}
