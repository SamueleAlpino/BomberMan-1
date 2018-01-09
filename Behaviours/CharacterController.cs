using Aiv.Fast2D;
using Aiv.Fast2D.Utils.Input;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BomberMan.GameObjects;
using OpenTK;

namespace BomberMan
{
    public class CharacterController : Behaviour, IUpdatable
    {
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }

        private bool canMoving;
        private bool moving;
        private Vector2 nextPos;
        private GameObject owner;
        private float vDist;


        public CharacterController(GameObject owner) : base(owner)
        {
            this.owner = owner;
            canMoving = true;
            moving = false;
        }
        public void Update()
        {
            SetDirection();
            owner.Transform.Position += Direction * Time.DeltaTime * Speed;
        }

        private void SetDirection()
        {
            if (Input.IsKeyPressed(KeyCode.S))
            {
                Direction = new Vector2(0, 1);
            }
            else if (Input.IsKeyPressed(KeyCode.W))
            {
                Direction = new Vector2(0, -1);
            }
            else if (Input.IsKeyPressed(KeyCode.A))
            {
                Direction = new Vector2(-1,0);
            }
            else if (Input.IsKeyPressed(KeyCode.D))
            {
                Direction = new Vector2(1, 0);
            }
            else
            {
                Direction = Vector2.Zero;
            }
        }
    }
}
