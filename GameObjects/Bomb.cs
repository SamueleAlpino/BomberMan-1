using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviourEngine;
using OpenTK;
using BehaviourEngine.Interfaces;
using BehaviourEngine.Renderer;
using Aiv.Fast2D.Utils.Input;

namespace BomberMan.GameObjects
{
    public class Bomb : GameObject
    {
        private Dictionary<string, AnimationRenderer> renderer;
        private List<AnimationRenderer> xplosion = new List<AnimationRenderer>();
        private List<Box2D> colliders = new List<Box2D>();
        private List<Vector2> locations = new List<Vector2>();
        private StateExplode explode;
        private StateWait wait;
        private IState currentState;

        public bool Exploding { get; private set; }

        public bool Stop
        {
            get; set;
        }
        public bool Show
        {
            get; set;
        }

        public Bomb(Vector2 spawnPosition) : base((int)RenderLayer.Pawn, "Bomb")
        {
            renderer = new Dictionary<string, AnimationRenderer>();
            renderer.Add("Bomb", new AnimationRenderer(this, FlyWeight.Get("Bomb"), 150, 150, 4, new int[] { 0, 1, 2, 3, 2 }, 0.2f, spawnPosition, true, false));

            //add behaviour for each value
            renderer.ToList().ForEach(x => AddBehaviour<AnimationRenderer>(x.Value));

            wait = new StateWait(this);
            explode = new StateExplode(this);

            explode.Next = wait;
            wait.Next = explode;

            wait.OnStateEnter();
            currentState = wait;

            AddBehaviour<UpdateBomb>(new UpdateBomb(this, currentState));
        }

        private class UpdateBomb : Behaviour, IUpdatable
        {
            private IState currentState;
            private Bomb owner;

            public UpdateBomb(Bomb owner, IState state) : base(owner)
            {
                this.owner = owner;
                this.currentState = state;
            }

            public void Update()
            {
                currentState = currentState.OnStateUpdate();
            }
        }

        public void SetAnimation(string animation, Vector2 direction)
        {
            renderer[animation].Owner.Transform.Position = direction;
        }

        public void EnableAnimation(bool bomb, string name, bool stop, bool render)
        {
            renderer[name].Stop = stop;
            renderer[name].Show = render;
        }

        public List<Vector2> GetAdjacentLocation(Vector2 from)
        {
            List<Vector2> adjacentLocation = new List<Vector2>();

            if (Map.GetIndex(true, (int)from.X, (int)from.Y))
                adjacentLocation.Add(new Vector2(from.X, from.Y));

            if (Map.GetIndex(true, (int)from.X - 1, (int)from.Y))
                adjacentLocation.Add(new Vector2(from.X - 1, from.Y));

            if (Map.GetIndex(true, (int)from.X, (int)from.Y - 1))
                adjacentLocation.Add(new Vector2(from.X, from.Y - 1));

            if (Map.GetIndex(true, (int)from.X + 1, (int)from.Y))
                adjacentLocation.Add(new Vector2(from.X + 1, from.Y));

            if (Map.GetIndex(true, (int)from.X, (int)from.Y + 1))
                adjacentLocation.Add(new Vector2(from.X, from.Y + 1));

            return adjacentLocation;
        }

        private class StateExplode : IState
        {
            public StateWait Next { get; set; }
            private Bomb owner { get; set; }
            private Timer timer;

            public StateExplode(Bomb owner)
            {
                this.owner = owner;
                timer = new Timer(3);
            }

            public void OnStateEnter()
            {
                timer.Start();
                owner.Exploding = true;
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                if (owner.Exploding)
                {
                    owner.locations = owner.GetAdjacentLocation(owner.Transform.Position);

                    owner.locations.ForEach(x => new Explosion(owner, x));

                    owner.Exploding = false;
                }

                if (timer.IsActive)
                    timer.Update();

                if (!timer.IsActive)
                {
                    Pool<Bomb>.RecycleInstance
                    (
                        owner, x =>
                        {
                            x.Active = false;
                        }
                    );
                    Next.OnStateEnter();
                    return Next;
                }

                return this;
            }
        }

        private class StateWait : IState
        {
            public StateExplode Next { get; set; }
            private Bomb owner { get; set; }
            private Timer timer;

            public StateWait(Bomb owner)
            {
                this.owner = owner;
                timer = new Timer(1f);
            }

            public void OnStateEnter()
            {
                timer.Start();
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                if (timer.IsActive)
                    timer.Update();

                owner.EnableAnimation(true, "Bomb", owner.Stop, owner.Show);

                if (!timer.IsActive)
                {
                    //owner.EnableAnimation(true, "Bomb", true, false);
                    Next.OnStateEnter();
                    return Next;
                }
                return this;
            }
        }
    }
}
