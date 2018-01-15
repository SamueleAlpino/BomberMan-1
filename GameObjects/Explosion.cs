using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviourEngine;
using BehaviourEngine.Renderer;
using OpenTK;
using BehaviourEngine.Interfaces;

namespace BomberMan.GameObjects
{
    public class Explosion : GameObject, IPhysical
    {
        public BoxCollider BoxCollider { get; set; }

        private AnimationRenderer anim;
        private UpdateCollider boxMng;
        private float lenght = 15f;

        public Explosion( Vector2 spawnPosition) : base((int)RenderLayer.Pawn, "Explosion")
        {
            this.Active = false;

            anim = new AnimationRenderer(this, FlyWeight.Get("Explosion"), 100, 100, 9, new int[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21, 22, 23, 24, 25, 26, 27, 28, 29, 30,
                31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
                41, 42, 43, 44, 45, 56, 57, 58, 59, 50,
                51, 52, 53, 54, 55, 56, 57, 58, 59, 60,
                61, 62, 63, 64, 65, 66, 67, 68, 69, 70,
                71, 72, 73, 74, 75
            }, lenght * Time.DeltaTime, spawnPosition, true, false);

            anim.UpdatePosition = true;
            BoxCollider = new BoxCollider(0.7f, 0.7f, this);
            AddBehaviour<BoxCollider>(BoxCollider);

            boxMng        = new UpdateCollider(this);
            boxMng.Offset = new Vector2(0.2f,0.2f);
            AddBehaviour<UpdateCollider>(boxMng);

            AddBehaviour<AnimationRenderer>(anim);

            Engine.AddPhysicalObject(this);
            Engine.Spawn(this);
        }

        public void Reset()
        {
            this.Active = false;
            anim.Reset();
        }

        public void OnIntersect(IPhysical other)
        {
            if (other is AI)
            {
                Pool<AI>.RecycleInstance(other as AI, x =>
                {
                    for (int i = 0; i < x.Behaviours.Count; i++)
                    {
                        x.Behaviours[i].Enabled = false;
                    }
                });
            }
        }

        public void OnTriggerEnter(IPhysical other)
        {
            throw new NotImplementedException();
        }
    }
}
