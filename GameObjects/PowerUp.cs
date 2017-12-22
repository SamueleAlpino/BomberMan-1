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

    public class PowerUp : GameObject, IPhysical, IPowerup
    {
        private SpriteRenderer renderer;
        private static readonly string[] powerUpsTextures;

        public BoxCollider BoxCollider { get; set; }

        private PowerUpType pType;

        //values for health and speed pUps
        private float[] speedRndValue;
        private int[] healthRndValue;

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
            renderer = new SpriteRenderer(powerUpsTextures[(int)type], this);
            AddBehaviour<SpriteRenderer>(renderer);

            this.Transform.Position = spawnPosition;

            BoxCollider = new BoxCollider(0.5f, 0.5f, this);
            AddBehaviour<BoxCollider>(BoxCollider);

            Engine.AddPhysicalObject(this);

            speedRndValue = new float[]
            {
                2.3f,
                3.2f,
                3.9f,
                1.4f,
                4.5f
            };

            healthRndValue = new int[]
            {
                2,
                4,
                3,
                2,
                5
            };
        }

        public void ApplyPowerUp(IPowerupable powerUp)
        {
            if (pType == PowerUpType.HEALTH)
                powerUp.ApplyHealth(healthRndValue[RandomManager.Instance.Random.Next(0, healthRndValue.Length)]); 
            else
                powerUp.ApplySpeed(healthRndValue[RandomManager.Instance.Random.Next(0, speedRndValue.Length)]);
        }

        public void OnIntersect(IPhysical other)
        {

        }
    }
}
