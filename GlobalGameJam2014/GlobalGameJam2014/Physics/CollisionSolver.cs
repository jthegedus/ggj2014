
namespace GGJ2014.Physics
{
    using System;
    using Microsoft.Xna.Framework;
    using GGJ2014.Interfaces;

    public static class CollisionSolver
    {
        public static void SolveCollision(IDynamic moveableObject, IDynamic moveableObject2)
        {
            throw new NotImplementedException("Moveable can't hit moveable");
        }

        public static Vector2 SolveCollision(IDynamic moveableObject, IStatic staticObject)
        {
            Rectangle rect1 = GeometryUtility.GetAdjustedRectangle(moveableObject.TransformComponent.Position, moveableObject.CollisionRectangle);
            Rectangle rect2 = GeometryUtility.GetAdjustedRectangle(staticObject.TransformComponent.Position, staticObject.CollisionRectangle);

            if (rect1.Intersects(rect2))
            {
                Vector2 movement = moveableObject.TransformComponent.Position - moveableObject.MovementComponent.LastPosition;
                Vector2 penetration = Vector2.Zero;
                if (movement.X > 0)
                {
                    penetration.X = rect1.Right - rect2.Left;
                    if (penetration.X <= 0 || penetration.X > movement.X + 1)
                    {
                        penetration.X = 0;
                    }
                }
                else if (movement.X < 0)
                {
                    penetration.X = rect1.Left - rect2.Right;
                    if (penetration.X >= 0 || penetration.X < movement.X - 1)
                    {
                        penetration.X = 0;
                    }
                }

                if (movement.Y > 0)
                {
                    penetration.Y = rect1.Bottom - rect2.Top;
                    if (penetration.Y <= 0 || penetration.Y > movement.Y + 1)
                    {
                        penetration.Y = 0;
                    }
                }
                else if (movement.Y < 0)
                {
                    penetration.Y = rect1.Top - rect2.Bottom;
                    if (penetration.Y >= 0 || penetration.Y < movement.Y - 1)
                    {
                        penetration.Y = 0;
                    }
                }

                return penetration;
            }

            return Vector2.Zero;
        }

        public static CorrectionVector2 SolveCollisionOther(IDynamic moveableObject, Rectangle staticObject)
        {
            Rectangle rect1 = moveableObject.CollisionRectangle;

            CorrectionVector2 penetration = new CorrectionVector2();
            penetration.X = 0;
            penetration.Y = 0;
            penetration.DirectionX = DirectionX.None;
            penetration.DirectionY = DirectionY.None;

            if (rect1.Intersects(staticObject) || staticObject.Intersects(rect1))
            {
                Vector2 movement = moveableObject.TransformComponent.Position - moveableObject.MovementComponent.LastPosition;

                float x1 = moveableObject.TransformComponent.Position.X + rect1.Width / 2 - staticObject.Left;
                float x2 = moveableObject.TransformComponent.Position.X - rect1.Width / 2 - staticObject.Right;
                float y1 = moveableObject.TransformComponent.Position.Y - rect1.Height / 2 - staticObject.Bottom;
                float y2 = moveableObject.TransformComponent.Position.Y + rect1.Height / 2 - staticObject.Top;

                penetration.Y = moveableObject.TransformComponent.Position.Y - rect1.Height / 2 - staticObject.Bottom;

                if (x1 < x2)
                {
                    penetration.X = x1;
                    penetration.DirectionX = DirectionX.Left;
                }
                else if (x1 > x2)
                {
                    penetration.X = x2;
                    penetration.DirectionX = DirectionX.Right;
                }

                // calculate displacement along Y-axis
                if (y1 < y2)
                {
                    penetration.Y = y1;
                    penetration.DirectionY = DirectionY.Up;
                }
                else if (y1 > y2)
                {
                    penetration.Y = y2;
                    penetration.DirectionY = DirectionY.Down;
                }

                return penetration;
            }

            return penetration;
        }

        public static Vector2 SolveCollision(IDynamic moveableObject, Rectangle staticObject)
        {
            Rectangle rect1 = moveableObject.CollisionRectangle;

            if (rect1.Intersects(staticObject) || staticObject.Intersects(rect1))
            {
                Vector2 movement = moveableObject.TransformComponent.Position - moveableObject.MovementComponent.LastPosition;
                Vector2 penetration = Vector2.Zero;
                if (movement.X > 0)
                {
                    penetration.X = moveableObject.TransformComponent.Position.X + rect1.Width / 2 - staticObject.Left;
                    if (penetration.X <= 0 || penetration.X > movement.X + 1)
                    {
                        penetration.X = 0;
                    }
                }
                else if (movement.X < 0)
                {
                    penetration.X = moveableObject.TransformComponent.Position.X - rect1.Width / 2 - staticObject.Right;
                    if (penetration.X >= 0 || penetration.X < movement.X - 1)
                    {
                        penetration.X = 0;
                    }
                }

                if (movement.Y > 0)
                {
                    penetration.Y = moveableObject.TransformComponent.Position.Y + rect1.Height / 2 - staticObject.Top;
                    if (penetration.Y <= 0 || penetration.Y > movement.Y + 1)
                    {
                        penetration.Y = 0;
                    }
                }
                else if (movement.Y < 0)
                {
                    penetration.Y = moveableObject.TransformComponent.Position.Y - rect1.Height / 2 - staticObject.Bottom;
                    if (penetration.Y >= 0 || penetration.Y < movement.Y - 1)
                    {
                        penetration.Y = 0;
                    }
                }

                return penetration;
            }

            return Vector2.Zero;
        }
    }
}
