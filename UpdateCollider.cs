using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using OpenTK;
using BehaviourEngine;
using BehaviourEngine.Interfaces;

namespace BomberMan
{
    public class UpdateCollider : Behaviour, IUpdatable
    {
        public Vector2 Offset;
        private BoxCollider collider;
        public UpdateCollider(GameObject owner) : base(owner)
        {
            collider = owner.GetComponent<BoxCollider>();
        }

        public void Update()
        {
            collider.Position = Owner.Transform.Position + Offset;
        }
    }
}
