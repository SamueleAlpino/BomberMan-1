using System.Collections.Generic;
using System.Linq;
using BehaviourEngine;
using OpenTK;
using BehaviourEngine.Interfaces;

namespace BomberMan.GameObjects
{
    public class Bomb : GameObject
    {
        public List<Explosion> explosionList = new List<Explosion>();
        private Dictionary<string, AnimationRenderer> renderer;
        private List<AnimationRenderer> xplosion = new List<AnimationRenderer>();
        private List<BoxCollider> colliders      = new List<BoxCollider>();
        private List<Vector2> locations          = new List<Vector2>();
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

            wait    = new StateWait(this);
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

        public void EnableAnimation(string name, bool stop, bool render)
        {
            renderer[name].Stop = stop;
            renderer[name].Show = render;
        }

        public static List<Vector2> GetAdjacentLocation(Vector2 from)
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
            private Explosion explosion;

            public StateExplode(Bomb owner)
            {
                this.owner = owner;
                timer = new Timer(3);
           //     explosion        = new Explosion(owner, Vector2.Zero);
           //     explosion.Active = false;
           //     Pool<Explosion>.Register(()=> explosion);
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
                    owner.locations = GetAdjacentLocation(owner.Transform.Position);

                    owner.locations.ForEach(x => explosion = new Explosion(x));

                    for (int i = 0; i < owner.locations.Count; i++)
                    {
                       explosion = Pool<Explosion>.GetInstance(x =>
                       {
                           x.Transform.Position = owner.locations[i];
                           x.Active = true;
                       });

                        owner.explosionList.Add(explosion);
                    }

                    owner.Exploding = false;
                }

                if (timer.IsActive)
                    timer.Update();

                if (!timer.IsActive)
                {
                    for (int i = 0; i < owner.explosionList.Count; i++)
                    {
                        Pool<Explosion>.RecycleInstance
                       (
                           owner.explosionList[i], x =>
                           {
                               x.Reset();
                           }
                       );
                    }

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

                owner.EnableAnimation("Bomb", owner.Stop, owner.Show);

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
