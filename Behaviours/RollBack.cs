using BehaviourEngine;
using BehaviourEngine.Interfaces;
using BomberMan.GameObjects;
using OpenTK;
using System;
using System.Collections.Generic;

namespace BomberMan.Behaviours
{
    public class RollBack : Behaviour, IUpdatable
    {
        public Vector2 Offset;

        private GameObject owner;
        private BoxCollider box;

        public RollBack(GameObject owner, BoxCollider box) : base(owner)
        {
            this.owner = owner;
            this.box = box;
        }

        public void Update()
        {
            if (box == null || owner == null) return;

            Vector2 oldPos = box.Position;

            box.Position = owner.Transform.Position + Offset;

            bool prev = CheckCollision((IPhysical)owner, Engine.PhysicalObjects);

            if (!prev) return;
            owner.Transform.Position = oldPos - Offset;
            box.Position = oldPos;
        }

        private bool CheckCollision(IPhysical box, List<IPhysical> boxes)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                IPhysical currentBox = boxes[i];

                if (box.BoxCollider != currentBox.BoxCollider 
                    && currentBox.GetType() != typeof(PowerUp) 
                    && currentBox.GetType() != typeof(AI) 
                    && currentBox.GetType() != typeof(Explosion))
                {
                    if (PhysicsManager.Intersect(box.BoxCollider, currentBox.BoxCollider))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
