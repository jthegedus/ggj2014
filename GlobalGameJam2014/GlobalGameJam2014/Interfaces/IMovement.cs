using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GGJ2014.Components;

namespace GGJ2014.Interfaces
{
    public interface IMovement : ITransform
    {
        MovementComponent MovementComponent { get; set; }
    }
}
