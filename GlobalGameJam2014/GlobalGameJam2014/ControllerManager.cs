using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.Interfaces;

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
    }
}
