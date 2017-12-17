using BehaviourEngine;
using BehaviourEngine.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Behaviours
{
    public class RollBack : Behaviour, IUpdatable
    {
        private GameObject owner;
        private BoxCollider box;

        public RollBack(GameObject owner, BoxCollider box) : base(owner)
        {
            this.owner = owner;
            this.box = box;
        }

        public void Update()
        {
            //TODO: need to refactor this rollback collision

            //if (box == null || owner == null) return;
            //Vector2 oldPos = box.Position;

            //box.Position = owner.Transform.Position;

            //bool prev = Engine.ComputeIntersect(Engine.levelColliders, owner.GetComponent<Box2D>());

            //if (!prev) return;
            //owner.Transform.Position = oldPos;
            //box.Position = oldPos;
        }
    }
}
