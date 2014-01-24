using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GGJ2014.Physics
{
    public static class Extensions
    {
        public static bool ApproxEquals(this Vector2 v1, Vector2 v2)
        {
            if (Vector2.DistanceSquared(v1, v2) < 0.01)
            {
                return true;
            }

            return false;
        }

        public static int Mod(this int a, int mod)
        {
            return (Math.Abs(a * mod) + mod) % mod;
        }
    }
}
