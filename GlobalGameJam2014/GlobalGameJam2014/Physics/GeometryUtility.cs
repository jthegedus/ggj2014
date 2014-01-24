
namespace GGJ2014.Physics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;

    public static class GeometryUtility
    {
        public static Rectangle GetAdjustedRectangle(Vector2 position, Rectangle rectangle)
        {
            return new Rectangle(
                (int)position.X - rectangle.Width / 2,
                (int)position.Y - rectangle.Height / 2,
                rectangle.Width,
                rectangle.Height);
        }
    }
}
