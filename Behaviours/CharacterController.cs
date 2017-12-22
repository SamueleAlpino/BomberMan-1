using Aiv.Fast2D;
using Aiv.Fast2D.Utils.Input;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BomberMan.GameObjects;
using OpenTK;

namespace BomberMan.Behaviours
{
    public class CharacterController : Behaviour, IUpdatable
    {
        private GameObject  owner;
        private bool        completed;
        private float       vDist;
        private Vector2     nextPos;
        private float       speed;


        public CharacterController(float speed, GameObject owner) : base(owner)
        {
            this.owner = owner;
            this.speed = speed;
        }

        public void Update()
        {
            if (Input.IsKeyPressed(KeyCode.W))
            {
                nextPos = GetNextLocationUp(owner.Transform.Position);
                completed = true;
            }

            else if(Input.IsKeyPressed(KeyCode.S))
            {
                nextPos = GetNextLocationDown(owner.Transform.Position);
                completed = true;
            }

            else if (Input.IsKeyPressed(KeyCode.A))
            {
                nextPos = GetNextLocationLeft(owner.Transform.Position);
                completed = true;
            }

            else if (Input.IsKeyPressed(KeyCode.D))
            {
                nextPos = GetNextLocationRight(owner.Transform.Position);
                completed = true;
            }

            if (completed)
            {
                vDist = (nextPos - owner.Transform.Position).Length;
                owner.Transform.Position = Vector2.Lerp(owner.Transform.Position, nextPos, Time.DeltaTime * speed);
                if (vDist < 1f)
                {
                    nextPos = Vector2.Zero;
                    completed = false;
                }
            }
        }

        private Vector2 GetNextLocationUp(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X, from.Y - 1);
            return from;
        }

        private Vector2 GetNextLocationDown(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X, from.Y + 1);
            return from;
        }

        private Vector2 GetNextLocationLeft(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X - 1, from.Y);
            return from;
        }

        private Vector2 GetNextLocationRight(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X + 1, from.Y);
            return from;
        }
    }
}
