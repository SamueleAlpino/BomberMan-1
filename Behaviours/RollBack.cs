﻿using BehaviourEngine;
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

            if (box == null || owner == null) return;

            Vector2 oldPos = box.Position;

            box.Position = owner.Transform.Position;

            bool prev = CheckCollision((IPhysical)owner, Engine.PhysicalObjects);

             if (!prev) return;
             owner.Transform.Position = oldPos;
              box.Position = oldPos;
        }

        private bool CheckCollision(IPhysical box, List<IPhysical> boxes)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                if (box.BoxCollider != boxes[i].BoxCollider)
                {
                    if (PhysicsManager.Intersect(box.BoxCollider, boxes[i].BoxCollider))
                    {
                        Console.WriteLine("ciaone proprio");
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
