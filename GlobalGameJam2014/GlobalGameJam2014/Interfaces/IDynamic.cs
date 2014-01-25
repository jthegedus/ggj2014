using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GGJ2014.Levels;

namespace GGJ2014.Interfaces
{
    public interface IDynamic : IMovement, IStatic
    {
        void HandleMapCollisions(Level level);
    }
}
