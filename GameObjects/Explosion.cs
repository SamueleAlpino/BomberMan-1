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
        private AnimationRenderer anim;
        public BoxCollider BoxCollider { get; set; }

        public Explosion( Vector2 spawnPosition) : base((int)RenderLayer.Pawn, "Explosion")
        {
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
            }, 15f * Time.DeltaTime, spawnPosition, true, false, false, Vector2.One * 1.1f);

            BoxCollider = new BoxCollider(0.7f, 0.7f, this);
            anim.UpdatePosition = true;

            this.AddBehaviour<AnimationRenderer>(anim);
            AddBehaviour<BoxCollider>(BoxCollider);

            Engine.AddPhysicalObject(this);
            Engine.Spawn(this);
        }


        public void OnIntersect(IPhysical other)
        {
        }
    }
}
