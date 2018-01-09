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

    public class PowerUpSpawner : GameObject
    {
        public PowerUpSpawner(int numberOfPowerUps) : base((int)RenderLayer.Pawn, "PowerUp")
        {
            AddBehaviour<PowerUpManager>(new PowerUpManager(numberOfPowerUps, this));
        }
    }

    public class PowerUp : GameObject, IPhysical, IPowerup
    {
        private SpriteRenderer renderer;
        private static readonly string[] powerUpsTextures;

        public BoxCollider BoxCollider { get; set; }

        private PowerUpType pType;

        //values for health and speed pUps
        private float[] speedRndValue;

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
            BoxCollider.Offset = new Vector2(0.25f,0.25f);
            AddBehaviour<BoxCollider>(BoxCollider);

            Engine.AddPhysicalObject(this);

            speedRndValue = new float[]
            {
                2.3f,
                3.1f,
                2.9f,
                1.4f,
                2.5f
            };
        }

        public void ApplyPowerUp(IPowerupable powerUp)
        {
            if (pType == PowerUpType.HEALTH)
                powerUp.ApplyHealth(RandomManager.Instance.Random.Next(2, 4)); 
            else
                powerUp.ApplySpeed(speedRndValue[RandomManager.Instance.Random.Next(0, speedRndValue.Length)]);
        }

        public void OnIntersect(IPhysical other)
        {

        }

        public void OnTriggerEnter(IPhysical other)
        {

        }
    }

    public class PowerUpManager : Behaviour
    {
        private PowerUp powerUp;

        public PowerUpManager(int size, GameObject owner) : base(owner)
        {
            for (int i = 0; i < size; i++)
            {
                int randomPowerType = RandomManager.Instance.Random.Next((int)PowerUpType.SPEED, (int)PowerUpType.HEALTH);
                powerUp = new PowerUp(Map.powerUpSpawnPoints[RandomManager.Instance.Random.Next(0, Map.powerUpSpawnPoints.Count)], (PowerUpType)randomPowerType);
                Engine.Spawn(powerUp);
            }
        }
    }
}
