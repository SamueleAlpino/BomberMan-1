using Aiv.Fast2D;
using Aiv.Fast2D.Utils.Input;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BomberMan.GameObjects;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Behaviours
{
    public class Controller : Behaviour, IUpdatable
    {
        private GameObject owner;
        private bool completed;
        float vDist;
        Vector2 nextPos;

        public Controller(GameObject owner) : base(owner)
        {
            this.owner = owner;
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
                owner.Transform.Position = Vector2.Lerp(owner.Transform.Position, nextPos, Time.DeltaTime * 5.0f);

                if (vDist < 0.3f)
                    completed = false;
            }
        }

        private static Vector2 GetNextLocationUp(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X, from.Y - 1);
            return Vector2.Zero;
        }

        private static Vector2 GetNextLocationDown(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X, from.Y + 1);
            return Vector2.Zero;
        }

        private static Vector2 GetNextLocationLeft(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X - 1, from.Y);
            return Vector2.Zero;
        }

        private static Vector2 GetNextLocationRight(Vector2 from)
        {
            if (Map.GetCellMove((int)from.X, (int)from.Y))
                return new Vector2(from.X + 1, from.Y);
            return Vector2.Zero;
        }
    }
}
