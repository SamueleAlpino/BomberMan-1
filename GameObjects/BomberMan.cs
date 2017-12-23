using BehaviourEngine;
using BehaviourEngine.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using BehaviourEngine.Interfaces;
using Aiv.Fast2D;
using Aiv.Fast2D.Utils.Input;
using BomberMan.Behaviours;

public struct Stats
{
    public float Speed;
    public int Health;

    public Stats(float speed, int health)
    {
        this.Speed = speed;
        this.Health = health;
    }

}

public interface IPowerupable
{
    void ApplySpeed(float amount);
    float ApplyHealth(int amount);
}

public interface IPowerup
{
    void ApplyPowerUp(IPowerupable powerUp);
}

namespace BomberMan.GameObjects
{
    public class Player : GameObject , IPhysical, IPowerupable
    {
        //Animation dictionary
        private Dictionary<string, AnimationRenderer> renderer;

        //fms drop bomb
        private StateDrop drop;

        //fms walk state
        private WalkUp walkUp;
        private WalkDown walkDown;
        private WalkLeft walkLeft;
        private WalkRight walkRight;
        private Idle idle;
        private IState walkState;
        private IState bombState;
        private List<IState> states;

        public BoxCollider BoxCollider { get; set; }

        //stats
        private Stats stat;
        private float speed;
        private int health;

        private CharacterController controller;

        public Player(string fileName, ref Stats stat, Vector2 drawPosition) : base((int)RenderLayer.Pawn, "BomberMan")
        {
            speed = stat.Speed;
            health = stat.Health;

            states = new List<IState>();

            renderer = new Dictionary<string, AnimationRenderer>();
            {
                renderer.Add("WalkRight" , new AnimationRenderer(this, FlyWeight.Get(fileName), 29, 28, 15, new int[] { 3, 18, 33 }, 0.09f, drawPosition, false, true));
                renderer.Add("WalkLeft"  , new AnimationRenderer(this, FlyWeight.Get(fileName), 29, 28, 15, new int[] { 1, 16, 31 }, 0.09f, drawPosition, false, true));
                renderer.Add("WalkDown"  , new AnimationRenderer(this, FlyWeight.Get(fileName), 29, 28, 15, new int[] { 0, 15, 30, 15 }, 0.09f, drawPosition, false, true));
                renderer.Add("WalkUp"    , new AnimationRenderer(this, FlyWeight.Get(fileName), 29, 28, 15, new int[] { 2, 17, 32 }, 0.09f, drawPosition, false, true));
                renderer.Add("Idle"      , new AnimationRenderer(this, FlyWeight.Get(fileName), 29, 28, 15, new int[] { 0 }, 0.09f, drawPosition, true, false));
            };
            renderer.ToList().ForEach(item => Transform.Position = drawPosition);

            renderer.ToList().ForEach(item => AddBehaviour<AnimationRenderer>(item.Value));

            //controller
            //  controller       = new CharacterController(this);
            //  controller.Speed = speed;

            AddBehaviour<Move>(new Move(this));
            BoxCollider = new BoxCollider(0.7f, 0.7f, this);
            AddBehaviour<BoxCollider>(BoxCollider);
            Engine.AddPhysicalObject(this);

            //init fsm
            drop = new StateDrop(this);

            walkUp = new WalkUp(this);
            walkDown = new WalkDown(this);
            walkLeft = new WalkLeft(this);
            walkRight = new WalkRight(this);
            idle = new Idle(this);

            //walk up
            walkUp.NextDown = walkDown;
            walkUp.NextLeft = walkLeft;
            walkUp.NextRight = walkRight;
            walkUp.NextIdle = idle;

            //walk down
            walkDown.NextUp = walkUp;
            walkDown.NextLeft = walkLeft;
            walkDown.NextRight = walkRight;
            walkDown.NextIdle = idle;

            //walk left
            walkLeft.NextUp = walkUp;
            walkLeft.NextDown = walkDown;
            walkLeft.NextRight = walkRight;
            walkLeft.NextIdle = idle;

            //walk right
            walkRight.NextLeft = walkLeft;
            walkRight.NextUp = walkUp;
            walkRight.NextDown = walkDown;
            walkRight.NextIdle = idle;

            //idle
            idle.NextDown = walkDown;
            idle.NextUp = walkUp;
            idle.NextLeft = walkLeft;
            idle.NextRight = walkRight;

            idle.OnStateEnter();
            walkState = idle;

            //bomb fsm
            drop.OnStateEnter();
            bombState = drop;

            //add states to collection
            states.Add(walkState);
            states.Add(bombState);

            AddBehaviour<UpdateStates>(new UpdateStates(this, states));
        //    AddBehaviour<CharacterController>(controller);
        }


        [Obsolete("Method is deprecated.")]
        public void SetAnimation(string animation, Vector2 direction) => renderer[animation].Owner.Transform.Position = direction;

        [Obsolete("This Method is deprecated,  use the other overload instead.")]
        private void EnableAnimation(string name, bool stop, bool render)
        {
            renderer[name].Stop = stop;
            renderer[name].Show = render;
        }

        private void EnableAnimation(string name, bool enable)
        {
            KeyValuePair<string, AnimationRenderer> first = renderer.Single(x => x.Key == name);
            first.Value.Show = enable; first.Value.Stop = !enable;

            IEnumerable<KeyValuePair<string, AnimationRenderer>> second = renderer.Where(x => x.Key != name);
            second.ToList().ForEach(x => 
            {
                x.Value.Show = !enable;
                x.Value.Stop = !enable;
            });
        }

        public void OnIntersect(IPhysical other)
        {
            if(other is Tile)
            {
                //TODO: player collision
            }

            if(other is IPowerup) 
            {
                //callback (:
                IPowerup powerup = other as IPowerup;
                powerup.ApplyPowerUp(this);
            }
        }

        public void ApplySpeed(float amount)
        {
            //track previous speed and sum it so we don't lose data
            float finalSpeed = speed += amount;
         //   this.GetComponent<CharacterController>().Speed = finalSpeed;
            this.GetComponent<Move>().Speed = finalSpeed;
        }

        public float ApplyHealth(int amount)
        {
            //track back previous amount and sum it
            return health += amount;
        }

        private class StateDrop : IState
        {
            private Player owner { get; set; }
            private Timer  timer;

            public StateDrop(Player owner)
            {
                this.owner = owner;
                timer      = new Timer(2f);
            }

            public void OnStateEnter()
            {
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                if (Input.IsKeyDown(KeyCode.Space) && !timer.IsActive)
                {
                   Engine.Spawn(Pool<Bomb>.GetInstance(x =>
                   {
                       x.Active = true;
                       x.Stop = false;
                       x.Show = true;
                       x.Transform.Position = new Vector2((int)owner.BoxCollider.Position.X, (int)owner.BoxCollider.Position.Y);
                   }));

                    timer.Start();
                }

                if (timer.IsActive)
                    timer.Update();

                return this;
            }
        }

        private class WalkLeft : IState
        {
            public WalkUp NextUp { get; set; }
            public WalkDown NextDown { get; set; }
            public WalkRight NextRight { get; set; }
            public Idle NextIdle { get; set; }
            private Player owner { get; set; }

            public WalkLeft(Player owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter() => OnStateUpdate();

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                owner.EnableAnimation("WalkLeft", true);
                return this;
            }
        }

        private class WalkRight : IState
        {
            public WalkUp NextUp { get; set; }
            public WalkDown NextDown { get; set; }
            public WalkLeft NextLeft { get; set; }
            public Idle NextIdle { get; set; }
            private Player owner { get; set; }

            public WalkRight(Player owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                OnStateUpdate();
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                owner.EnableAnimation("WalkRight", true);
                return this;
            }
        }

        private class WalkUp : IState
        {
            public WalkLeft NextLeft { get; set; }
            public WalkDown NextDown { get; set; }
            public WalkRight NextRight { get; set; }
            public Idle NextIdle { get; set; }
            private Player owner { get; set; }

            public WalkUp(Player owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                OnStateUpdate();
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                owner.EnableAnimation("WalkUp", true);
                return this;
            }
        }

        private class WalkDown : IState
        {
            public WalkUp NextUp { get; set; }
            public WalkLeft NextLeft { get; set; }
            public WalkRight NextRight { get; set; }
            public Idle NextIdle { get; set; }
            private Player owner { get; set; }

            public WalkDown(Player owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                OnStateUpdate();
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                owner.EnableAnimation("WalkDown", true);
                return this;
            }
        }

        private class Idle : IState
        {
            public WalkUp NextUp { get; set; }
            public WalkDown NextDown { get; set; }
            public WalkRight NextRight { get; set; }
            public WalkLeft NextLeft { get; set; }
            private Player owner { get; set; }

            public Idle(Player owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
                OnStateUpdate();
            }

            public void OnStateExit()
            {

            }

            public IState OnStateUpdate()
            {
                if (Input.IsKeyPressed(KeyCode.S))
                {
                    NextDown.OnStateEnter();
                    return NextDown;
                }

                else if (Input.IsKeyPressed(KeyCode.W))
                {
                    NextUp.OnStateEnter();
                    return NextUp;
                }

                else if (Input.IsKeyPressed(KeyCode.D))
                {
                    NextRight.OnStateEnter();
                    return NextRight;
                }

                else if (Input.IsKeyPressed(KeyCode.A))
                {
                    NextLeft.OnStateEnter();
                    return NextLeft;
                }
                else
                {
                    owner.EnableAnimation("Idle", true);
                    return this;
                }
            }
        }
    }
}
