using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Fast2D;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
namespace BomberMan
{
    public class OnAABBChecker : Behaviour, IUpdatable
    {
        private BoxCollider toCheck;
        public OnAABBChecker(GameObject owner) : base(owner)
        {
            try
            {
                toCheck = owner.GetComponent<BoxCollider>();
            }
            catch (Exception e)
            {
                e = new Exception("This game object don't contain any box collider");
            }
        }

        public void Update()
        {
            for (int i = 0; i < Engine.PhysicalObjects.Count; i++)
            {
                if (toCheck != null && toCheck != Engine.PhysicalObjects[i].BoxCollider)
                {
                    HitState hitState = PhysicsManager.OnAABB(toCheck, Engine.PhysicalObjects[i].BoxCollider);
                    float offset = 0.03f;
                    if (hitState.hit)
                    {
                        if (hitState.normal.Y > 0f)
                        {
                            //collisione con parete by
                            Owner.Transform.Position.Y = Engine.PhysicalObjects[i].BoxCollider.Position.Y + Engine.PhysicalObjects[i].BoxCollider.Height + offset;
                        }
                        else if (hitState.normal.Y < 0f)
                        {
                            //collisione con parete ty
                            Owner.Transform.Position.Y = Engine.PhysicalObjects[i].BoxCollider.Position.Y - toCheck.Height - offset;
                        }
                        else if (hitState.normal.X > 0f)
                        {
                            //collisione con parete dx
                            Owner.Transform.Position.X = Engine.PhysicalObjects[i].BoxCollider.Position.X + Engine.PhysicalObjects[i].BoxCollider.Width + offset;
                        }
                        else if (hitState.normal.X < 0f)
                        {
                            //collisione con parete sx
                            Owner.Transform.Position.X = Engine.PhysicalObjects[i].BoxCollider.Position.X - toCheck.Width - offset;
                        }
                        break;
                    }
                }
            }
        }
    }
}
