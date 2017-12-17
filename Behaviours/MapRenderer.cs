using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviourEngine;
using BehaviourEngine.Interfaces;
using OpenTK;
using Aiv.Fast2D;

namespace BomberMan
{
    public class MapRenderer : Behaviour, IDrawable
    {
        private List<int> cells;
        private int columns;
        private Sprite sprite;
        public MapRenderer(List<int> cells, int columns, GameObject owner) : base(owner)
        {
            this.cells   = cells;
            this.columns = columns;
            sprite       = new Sprite(1, 1);

        }
        public void Draw()
        {
            for (int i = 0; i < cells.Count; i++)
            {

                if (cells[i] == 2)
                {
                    sprite.position = new Vector2(i % (columns - 1), i / (columns - 1));
                    sprite.DrawTexture(FlyWeight.Get("Obstacle"));
                }
                else if (cells[i] == 3)
                {
                    sprite.position = new Vector2(i % (columns - 1), i / (columns - 1));
                    sprite.DrawTexture(FlyWeight.Get("Wall"));
                }
            }
        }
    }
}
