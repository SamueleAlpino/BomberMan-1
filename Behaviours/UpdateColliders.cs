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
    public class UpdateColliders : Behaviour, IUpdatable
    {
        private BoxCollider collider;
        private Vector2 offset;

        public UpdateColliders(BoxCollider collider, GameObject owner, Vector2 offset) : base(owner)
        {
            this.collider = collider;
            this.offset = offset;
        }

        public void Update()
        {
            if (collider != null)
                collider.Position = Owner.Transform.Position + offset;
        }
    }
}
