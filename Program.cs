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

            new PowerUp(Vector2.Zero, PowerUpType.FIRE);

            Engine.Spawn(new Game());

            Engine.Run();
        }
    }
}
