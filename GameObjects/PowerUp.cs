﻿using BehaviourEngine;
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
        public BoxCollider BoxCollider { get; set; }
        public  readonly string[] PowerUpsTextures;

        public PowerUpType pType;

        private SpriteRenderer renderer;
        private UpdateCollider boxMng;
        //values for health and speed pUps
        private float[] speedRndValue;

        public PowerUp(Vector2 spawnPosition) : base((int)RenderLayer.Pawn, "Powerup")
        {
            PowerUpsTextures = new string[]
            {
                "Speed",
                "Health",
            };

            renderer = new SpriteRenderer(PowerUpsTextures[(int)pType], this);
            AddBehaviour<SpriteRenderer>(renderer);

            this.Transform.Position = spawnPosition;

            BoxCollider = new BoxCollider(0.5f, 0.5f, this);
           
            AddBehaviour<BoxCollider>(BoxCollider);

            boxMng = new UpdateCollider(this);
            boxMng.Offset = new Vector2(0.25f, 0.25f);
            AddBehaviour<UpdateCollider>(boxMng);
            Engine.AddPhysicalObject(this);

            speedRndValue = new float[]
            {
                1.3f,
                1.5f,
                1.9f,
                1.4f,
                1.5f
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
        public PowerUpManager(int size, GameObject owner) : base(owner)
        {
            for (int i = 0; i < size; i++)
            {
                Engine.Spawn(Pool<PowerUp>.GetInstance(x =>
                {
                    x.Transform.Position = Map.powerUpSpawnPoints[RandomManager.Instance.Random.Next(0, Map.powerUpSpawnPoints.Count)];
                    x.pType = (PowerUpType)RandomManager.Instance.Random.Next(0, Enum.GetNames(typeof(PowerUpType)).Length);
                    if(x.pType == PowerUpType.HEALTH)
                        x.GetComponent<SpriteRenderer>().SetTexture(x.PowerUpsTextures[(int)PowerUpType.HEALTH]);
                    else
                        x.GetComponent<SpriteRenderer>().SetTexture(x.PowerUpsTextures[(int)PowerUpType.SPEED]);

                    for (int component = 0; component < x.Behaviours.Count; component++)
                    {
                        x.Behaviours[component].Enabled = true;
                    }
                }));
            }
        }
    }
}
