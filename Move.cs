using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using Aiv.Fast2D.Utils.Input;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BomberMan.GameObjects;
using OpenTK;

namespace BomberMan
{
    public class Move : Behaviour, IUpdatable
    {
        public float Speed { get; set; }

        private bool canMoving;
        private bool moving;
        private Vector2 nextPos;
        private GameObject owner;
        private float vDist;


        public Move(GameObject owner) : base(owner)
        {
            this.owner = owner;
            canMoving  = true;
            moving     = false;
            Speed      = 5f;
        }

        public void Update()
        {
            if (canMoving)
            {
                nextPos = owner.Transform.Position;
                MovePlayer();
            }

            if (moving)
            {
                vDist = (nextPos - owner.Transform.Position).Length;
                if (vDist < 0.005f)
                {
                    moving    = false;
                    canMoving = true;
                }
                owner.Transform.Position = Vector2.Lerp(owner.Transform.Position, nextPos, Time.DeltaTime * Speed);
            }

         
        }

        private void MovePlayer()
        {
            if (Input.IsKeyPressed(KeyCode.W))
            {
                canMoving = false;
                moving    = true;
                nextPos   = GetNextLocationUp(owner.Transform.Position);
            }
            else if (Input.IsKeyPressed(KeyCode.S))
            {
                canMoving = false;
                moving = true;
                nextPos = GetNextLocationDown(owner.Transform.Position);
            }
            else if (Input.IsKeyPressed(KeyCode.A))
            {
                canMoving = false;
                moving = true;
                nextPos = GetNextLocationLeft(owner.Transform.Position);
            }
            else if (Input.IsKeyPressed(KeyCode.D))
            {
                canMoving = false;
                moving = true;
                nextPos = GetNextLocationRight(owner.Transform.Position);
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
