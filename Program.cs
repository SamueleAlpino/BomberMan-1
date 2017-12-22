using BehaviourEngine;
using BomberMan.Behaviours;
using BomberMan.GameObjects;
using OpenTK;

namespace BomberMan
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Init(1255, 600, "BomberMan", 11);

            Engine.Spawn(new Game());

            Engine.Run();
        }
    }
}
