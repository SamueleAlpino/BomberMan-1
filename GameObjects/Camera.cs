using BehaviourEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan
{
    public class Camera : GameObject
    {
        public Camera() : base((int)RenderLayer.None, "Main Camera")
        {
            AddBehaviour<CameraManager>(CameraManager.Instance);
        }
    }
}
