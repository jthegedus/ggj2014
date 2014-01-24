using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.GameObjects;
using GGJ2014.Interfaces;
using Microsoft.Xna.Framework;

namespace GGJ2014
{
    public class BulletPool : IUpdate
    {
        private const int Size = 100;
        private List<Bullet> active;
        private Stack<Bullet> inactive;

        public BulletPool()
        {
            active = new List<Bullet>(Size);
            inactive = new Stack<Bullet>(Size);
        }

        public void createBullet(Vector2 ownerPosition, Vector2 direction, Color owner, Vector2 initialVelocity)
        {
            Bullet newBullet;
            if (inactive.Count == 0)
            {
                newBullet = new Bullet();
            }
            else
            {
                newBullet = inactive.Pop();
            }
            newBullet.Initialize(ownerPosition, direction, owner, initialVelocity);
            TheyDontThinkItBeLikeItIsButItDo.WorldManager.AddToWorld(newBullet);
            active.Add(newBullet);
        } 
    
        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = active.Count; i > 0; --i)
            {
                if (active[i - 1].Lifespan <= 0)
                {
                    // remove from active & push inactive
                    inactive.Push(active[i - 1]);
                    // REMOVE FROM WORLD MANAGER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    TheyDontThinkItBeLikeItIsButItDo.WorldManager.RemoveFromWorld(active[i - 1]);
                    active.RemoveAt(i - 1);
                }
            }
        }
    }
}
