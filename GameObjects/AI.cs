using Aiv.Fast2D.Utils.Input;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BehaviourEngine.Pathfinding;
using BehaviourEngine.Renderer;
using BomberMan.Behaviours;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BomberMan.GameObjects
{
    public class SpawnManager : GameObject
    {
        public SpawnManager(IMap map, GameObject target) : base((int)RenderLayer.Pawn)
        {
            this.AddBehaviour<AIManager>(new AIManager(map, target));
        }
    }

    public class AI : GameObject, IPathfind , IPhysical
    {
        public Transform      Target;
        public List<Node>     CurrentPath { get; set; }
        public bool           Computed
        {
            get
            {
                if ((CurrentPath.Count) == 0)
                    return true;
                return false;
            }
        }

        public BoxCollider BoxCollider { get; set; }

        public float          Speed = 1.4f;

        private AnimationRenderer   renderer;
        private PatrolState         patrol;
        private StateIdle           idle;
        private ChaseState          chase;
        private IState              currentState;
        private IMap                map;
        private List<IState>        states = new List<IState>();

        public AI(Vector2 spawnPos, IMap map, Transform target) : base((int)RenderLayer.Pawn, "AI")
        {
            this.map    = map;
            this.Target = target;
            renderer = new AnimationRenderer(this, FlyWeight.Get("Balloon"), ((int)(float)Math.Floor(18.5m)), 17, 4, new int[] { 0, 1, 2, 3 }, 0.2f, spawnPos, true, false);
            renderer.Owner.Transform.Position = spawnPos;

            BoxCollider = new BoxCollider(0.7f, 0.7f, this);
            BoxCollider.Offset = new Vector2(0.2f, 0.2f);
            Engine.AddPhysicalObject(this);
            AddBehaviour<BoxCollider>(BoxCollider);

            AddBehaviour<AnimationRenderer>(renderer);

            patrol = new PatrolState(this);
            idle   = new StateIdle(this);
            chase  = new ChaseState(this);

            //chase.PatrolState = patrol;
            
            patrol.ChaseState = chase;
            chase.NextPatrol = patrol;
            

            patrol.OnStateEnter();
            currentState = patrol;

            states.Add(currentState);

            AddBehaviour<UpdateStates>(new UpdateStates(this, states));
       //     AddBehaviour<UpdateColliders>(new UpdateColliders(BoxCollider, this, new Vector2(.15f, .12f)));
        }

        private class CollisionHandler : Behaviour, IUpdatable
        {
            private GameObject owner;

            public CollisionHandler(GameObject owner) : base(owner)
            {
                this.owner = owner;
            }

            public void Update()
            {
                //if(Engine.ComputeIntersect(Engine.Boxes, this.owner.GetComponent<Box2D>()))
                //{
                //    Console.WriteLine("collided!");
                //}
            }
        }

        public void ComputePath<T>(T item, int x, int y) where T : IMap
        {
            int pX, pY;

            if (Math.Abs(Transform.Position.X - (int)Transform.Position.X) > 0.5f)
                pX = (int)Transform.Position.X + 1;
            else
                pX = (int)Transform.Position.X;

            if (Math.Abs(Transform.Position.Y - (int)Transform.Position.Y) > 0.5f)
                pY = (int)Transform.Position.Y + 1;
            else
                pY = (int)Transform.Position.Y;

            CurrentPath = AStar.GetPath(item, pX, pY, x, y);
        }

        public void OnIntersect(IPhysical other)
        {
            if (other is Player)
            {
                Console.WriteLine(this.ToString() + "Collided With:" + other.ToString());
            }
        }

        private class ChaseState : IState
        {
            public PatrolState  NextPatrol {get; set; }
            public ChaseState   NextChase {get; set; }
            public StateIdle    NextIdle { get; set; }

            private AI owner { get; set; }
            public ChaseState(AI owner)
            {
                this.owner = owner;
            }
            public void OnStateEnter()
            {
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                return this;
            }
        }
        private class PatrolState : IState
        {
            private AI owner;
            private Timer t;
            public ChaseState ChaseState;

            public PatrolState(AI owner)
            {
                this.owner = owner;
                t = new Timer(1.5f);
            }

            public void OnStateEnter()
            {
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                owner.ComputePath(owner.map, (int)owner.Target.Position.X, (int)owner.Target.Position.Y);

                if (owner.CurrentPath == null)
                    return this;
           
                if (owner.CurrentPath.Count == 0)
                {
                    owner.CurrentPath = null;
                    return this;
                }
           
                if(!owner.Computed)
                {
                    Vector2 targetPos = owner.CurrentPath[0].Position;
                    if (targetPos != owner.Transform.Position)
                    {
                        Vector2 direction = (targetPos - owner.Transform.Position).Normalized();
                        owner.Transform.Position += direction * owner.Speed * Time.DeltaTime;
                    }
           
                    float distance = (targetPos - owner.Transform.Position).Length;

                    if (distance <= 0.1f)
                        owner.CurrentPath.RemoveAt(0);

                }
                return this;
            }
        }

        private class StateIdle : IState
        {
            private AI owner;
            public StateIdle(AI owner)
            {
                this.owner = owner;
            }

            public void OnStateEnter()
            {
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {
                return this;
            }
        }
    }

    public class AIManager : Behaviour
    {
        private GameObject  owner;
        public  AI          enemies;
        private byte        columnsID = 23;

        public AIManager(IMap map, GameObject owner) : base(owner)
        {
            this.owner = owner;
            int[] ids  = ((map as Map).CellsID.ToArray());

            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == 12)
                    Engine.Spawn(new AI(new Vector2(i % columnsID, i / columnsID), map, owner.Transform));
            }
        }
    }
}
