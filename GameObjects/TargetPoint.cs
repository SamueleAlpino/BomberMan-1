using BehaviourEngine;
using BehaviourEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace BomberMan.GameObjects
{
    public class TargetPoint : GameObject, IWaypoint
    {
        public Vector2 Location { get => this.Transform.Position; set => this.Transform.Position = value; }

        public TargetPoint() : base((int)RenderLayer.None, "TargetPoint")
        {
            Location = Map.powerUpSpawnPoints[RandomManager.Instance.Random.Next(0, Map.powerUpSpawnPoints.Count)];
            //AddBehaviour<SpriteRenderer>(new SpriteRenderer("Bomb", this));
        }
    }

    public class TargetSpawner : GameObject
    {
        public TargetSpawner(int size, float shuffleTimeStep) : base((int)RenderLayer.None, "TargetSpawner") => AddBehaviour<TargetPointBehaviour>(new TargetPointBehaviour(size, shuffleTimeStep, this));
    }

    public class TargetPointBehaviour : Behaviour, IUpdatable
    {
        private float tMin;
        private float tMax;
        public TargetPoint current;

        public TargetPointBehaviour(int size, float shuffleTimeStep, GameObject owner) : base(owner)
        {
            tMax = shuffleTimeStep;

            for (int i = 0; i < size; i++)
            {
                current = new TargetPoint();
                GameManager.AddTargetPoint(current);
                Engine.Spawn(current);
            }
        }

        public void Update()
        {
            if (!Enabled) return;

            tMin += Time.DeltaTime;
            if (tMin > tMax)
            {
                GameManager.GetAllPoints().Where( x => x.GetType() != typeof(Player)).ToList().ForEach(item => (item as GameObject).GetComponent<Transform>().Position = Map.powerUpSpawnPoints[RandomManager.Instance.Random.Next(0, Map.powerUpSpawnPoints.Count)]);
                ResetTiming();
            }
        }

        private void ResetTiming()
        {
            this.tMin = 0.0f;
        }
    }
}
