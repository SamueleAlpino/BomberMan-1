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
        public TargetSpawner(int size) : base((int)RenderLayer.None, "TargetSpawner") => AddBehaviour<TargetPointBehaviour>(new TargetPointBehaviour(size, this));
    }

    public class TargetPointBehaviour : Behaviour
    {
        public TargetPoint current;

        public TargetPointBehaviour(int size, GameObject owner) : base(owner)
        {

            for (int i = 0; i < size; i++)
            {
                current = new TargetPoint();
                GameManager.AddTargetPoint(current);
                Engine.Spawn(current);
            }
        }
    }
}
