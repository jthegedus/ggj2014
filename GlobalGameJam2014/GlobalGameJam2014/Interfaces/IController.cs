using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GGJ2014.GameObjects;

namespace GGJ2014.Interfaces
{
    public interface IController
    {
        void HandleInput();
        void DamagedPlayer(Agent victim);
        void KilledPlayer(Agent victim);
        void BumpedPlayer(Agent victim);
        void CollectedCollectible();
    }
}
