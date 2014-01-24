using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGJ2014.Interfaces
{
    public interface IDynamic : IMovement, IStatic
    {
        void HandleMapCollisions();
    }
}
