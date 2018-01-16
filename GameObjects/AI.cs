using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BehaviourEngine.Pathfinding;
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

    public class AI : GameObject, IPathfind, IPhysical, IWaypoint
    {
        public List<Node> CurrentPath { get; set; }
        public bool Computed
        {
            get
            {
                if ((CurrentPath.Count) == 0)
                    return true;
                return false;
            }
        }
        public Vector2 Offset;
        public BoxCollider BoxCollider { get; set; }
        public float Speed = 1.4f;
        public IMap iMap
        {
            get
            {
                return map;
            }
            set
            {
                map = value;
            }
        }

        private List<IWaypoint> TargetPoints { get; set; }

        public IWaypoint Player { get; set; }
        public IWaypoint CurrentTarget { get; set; }
        public Vector2 Location { get => this.Transform.Position; set => this.Transform.Position = value; }

        private AnimationRenderer renderer;
        private PatrolState patrol;
        private StateIdle idle;
        private ChaseState chase;
        private IState currentState;
        private IMap map;
        private List<IState> states = new List<IState>();
        private UpdateCollider boxMng;
        private float radius;

        public AI(Vector2 spawnPos, IMap map, List<IWaypoint> targetPoints, Vector2 OffsetTarget, float radius, IWaypoint firstTarget) : base((int)RenderLayer.Pawn, "AI")
        {
            this.map = map;
            this.TargetPoints = targetPoints;
            this.Offset = OffsetTarget;
            this.radius = radius;
            CurrentTarget = firstTarget;
            renderer = new AnimationRenderer(this, FlyWeight.Get("Balloon"), ((int)(float)Math.Floor(18.5f)), 17, 4, new int[] { 0, 1, 2, 3 }, 0.2f, spawnPos, true, false);
            renderer.Owner.Transform.Position = spawnPos;

            BoxCollider = new BoxCollider(0.8f, 0.8f, this);

            Engine.AddPhysicalObject(this);
            AddBehaviour<BoxCollider>(BoxCollider);

            AddBehaviour<AnimationRenderer>(renderer);

            patrol = new PatrolState(this);
            idle = new StateIdle(this);
            chase = new ChaseState(this);

            //chase.PatrolState = patrol;

            patrol.ChaseState = chase;
            chase.NextPatrol = patrol;

            patrol.OnStateEnter();
            currentState = patrol;

            states.Add(currentState);

            boxMng = new UpdateCollider(this);
            boxMng.Offset = new Vector2(0.1f, 0.1f);
            AddBehaviour<UpdateCollider>(boxMng);
            AddBehaviour<UpdateStates>(new UpdateStates(this, states));
        }

        public bool IsInRadius(out GameObject target)
        {
            float distance = ((Player as GameObject).Transform.Position - this.Transform.Position).Length;

            if (distance < radius)
            {
                target = Player as GameObject;
                return true;
            }

            target = null;
            return false;
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
            if (other is Explosion)
            {
                AudioManager.PlayClip(AudioType.SOUND_DIE);
            }
        }

        public void OnTriggerEnter(IPhysical other)
        {

        }

        private class ChaseState : IState
        {
            public PatrolState NextPatrol { get; set; }
            public ChaseState NextChase { get; set; }
            public StateIdle NextIdle { get; set; }

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
            public  ChaseState ChaseState;
            private GameObject target;
            private IWaypoint next;
            public PatrolState(AI owner)
            {
                this.owner = owner;
                next = owner.CurrentTarget;
            }

            public void OnStateEnter()
            {
            }

            public void OnStateExit()
            {
            }

            public IState OnStateUpdate()
            {

                if (owner.IsInRadius(out target))
                {
                    if (target is Player) // cast not failed
                    {
                        owner.CurrentTarget = target as IWaypoint;
                        owner.ComputePath(owner.map, (int)((owner.CurrentTarget as Player).Transform.Position.X + owner.Offset.X), (int)((owner.CurrentTarget as Player).Transform.Position.Y + owner.Offset.Y));
                        Console.WriteLine("Current Target in Radius : {0}", owner.CurrentTarget);
                    }
                }
                else
                {
                    if ((owner.CurrentTarget.Location - owner.Transform.Position).Length < 1f)
                    {
                        next = GameManager.GetAllPoints()[RandomManager.Instance.Random.Next(0, GameManager.PointsCount)];
                        Console.WriteLine(next);

                        while (next is Player)
                        {
                            next = GameManager.GetAllPoints()[RandomManager.Instance.Random.Next(0, GameManager.PointsCount)];
                        }
                    }

                    owner.CurrentTarget = next;

                    if (owner.CurrentTarget is TargetPoint) //cast not failed
                        owner.ComputePath(owner.map, (int)((owner.CurrentTarget as TargetPoint).Transform.Position.X + owner.Offset.X), (int)((owner.CurrentTarget as TargetPoint).Transform.Position.Y + owner.Offset.Y));

                }

                if (owner.CurrentPath == null)
                    return this;

                if (owner.CurrentPath.Count == 0)
                {
                    owner.CurrentPath = null;
                    return this;
                }

                if (!owner.Computed)
                {
                    Vector2 targetPos = owner.CurrentPath[0].Position;
                    if (targetPos != owner.Transform.Position)
                    {
                        Vector2 direction = (targetPos - owner.Transform.Position).Normalized();
                        owner.Transform.Position += direction * 1.5f * Time.DeltaTime;
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
        private GameObject owner;
        public AI enemies;
        private byte columnsID = 23;

        public AIManager(IMap map, GameObject owner) : base(owner)
        {
            this.owner = owner;
            int[] ids = ((map as Map).CellsID.ToArray());

            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == 12)
                {
                    Engine.Spawn(Pool<AI>.GetInstance((x) =>
                   {
                       x.Transform.Position = new Vector2(i % columnsID, i / columnsID);
                       x.iMap = map;
                       x.Player = (owner as IWaypoint);
                       x.Offset = new Vector2(0.5f, 0.5f);

                       for (int component = 0; component < x.Behaviours.Count; component++)
                       {
                           x.Behaviours[component].Enabled = true;
                       }

                   }));
                }
            }
        }
    }
}
