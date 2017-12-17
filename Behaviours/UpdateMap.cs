using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviourEngine.Interfaces;
using BehaviourEngine;
using OpenTK;
using BehaviourEngine.Renderer;

namespace BomberMan
{
    public class UpdateMap :Behaviour, IUpdatable 
    {
        public List<int> Cells;

        public List<int> cells;

        private Tile tile;
        public UpdateMap(GameObject owner, List<int> cells, int columns) : base(owner)
        {
            this.cells = cells;
            Cells      = cells;
            Pool<BoxCollider>.Register(() => new BoxCollider( Vector2.Zero, 1, 1, Owner), 100);

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] == 2 || cells[i] == 3)
                {
                    tile = new Tile(new Vector2(i % (columns - 1), i / (columns - 1)));
                    Engine.AddPhysicalObject(tile);
                    GameObject t = Engine.Spawn(tile);
                }
           
            }
        }

        public void Update()
        {
           this.cells = Cells;
        }
    }
}
