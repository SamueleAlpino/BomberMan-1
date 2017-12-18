using BehaviourEngine;
using BomberMan.Behaviours;

namespace BomberMan
{
    class Program
    {
        static void Main(string[] args)
        {
            //lol
            Engine.Init(1255, 600, "BomberMan", 11);

            Engine.Spawn(new Game());

            Engine.Run();
        }
    }
}
