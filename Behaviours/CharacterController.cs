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

        private GameObject owner;
        private bool play;

        public CharacterController(GameObject owner) : base(owner)
        {
            this.owner = owner;
            play = false;
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
                SetDirection(new Vector2(0, 1), AudioType.SOUND_WALK_SLOW);
            }
            else if (Input.IsKeyPressed(KeyCode.W))
            {
                SetDirection(new Vector2(0, -1), AudioType.SOUND_WALK_SLOW);
            }
            else if (Input.IsKeyPressed(KeyCode.A))
            {
                SetDirection(new Vector2(-1, 0), AudioType.SOUND_WALK_SLOW);
            }
            else if (Input.IsKeyPressed(KeyCode.D))
            {
                SetDirection(new Vector2(1, 0), AudioType.SOUND_WALK_SLOW);
            }
            else
            {
                Direction = Vector2.Zero;
                play = false;
            }

            if (!play)
            {
                AudioManager.Pause(AudioType.SOUND_WALK_SLOW);
            }
        }

        private void SetDirection(Vector2 dir, AudioType type)
        {
            Direction = dir;
            AudioManager.PlayClip(type);
            play = false;
        }
    }
}
