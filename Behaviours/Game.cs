using BehaviourEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Behaviours
{
    public sealed class Game : GameObject
    {
        public Game() : base((int)RenderLayer.None)
        {
            Engine.Spawn(GameManager.Instance);
        }
    }
}
