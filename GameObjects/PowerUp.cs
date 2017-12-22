using BehaviourEngine;
using BehaviourEngine.Interfaces;
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
    }

    public class PowerUp : GameObject, IPhysical, IPowerup
    {
        private SpriteRenderer renderer;
        private static readonly string[] powerUpsTextures;

        public BoxCollider BoxCollider { get; set; }

        private PowerUpType pType;

        static PowerUp() //ok
        {
            powerUpsTextures = new string[]
            {
                "Speed",
                "Health",
            };
        }

        public PowerUp(Vector2 spawnPosition, PowerUpType type) : base((int)RenderLayer.Pawn, "Powerup")
        {
            this.pType = type;
            renderer = new SpriteRenderer("Bomb", this);
            AddBehaviour<SpriteRenderer>(renderer);

            this.Transform.Position = spawnPosition;

            BoxCollider = new BoxCollider(0.5f, 0.5f, this);
            AddBehaviour<BoxCollider>(BoxCollider);

            Engine.AddPhysicalObject(this);
        }

        public void ApplyPowerUp(IPowerupable mod)
        {
            if (pType == PowerUpType.HEALTH)
                Console.WriteLine(mod.ApplyHealth(10)); 
            else
                mod.ApplySpeed(10.0f);
        }

        public void OnIntersect(IPhysical other)
        {
            //TODO: can also be ignored
        }
    }
}
