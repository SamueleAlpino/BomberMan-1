using BehaviourEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.GameObjects
{
    public enum PowerUpType
    {
        SPEED = 0,
        HEALTH = 1,
        FIRE = 2
    }

    public class PowerUp : GameObject
    {
        private SpriteRenderer renderer;
        private static readonly string[] powerUpsTextures;

        static PowerUp()
        {
            powerUpsTextures = new string[]
            {
                "Speed",
                "Health",
            };
        }

        public PowerUp(Vector2 spawnPosition, PowerUpType type) : base((int)RenderLayer.Pawn, "Powerup")
        {
            renderer = new SpriteRenderer(powerUpsTextures[(int)type], this);

            Console.WriteLine(type);
            //AddBehaviour<SpriteRenderer>(renderer);
        }
    }
}
