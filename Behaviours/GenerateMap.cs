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
    public class GenerateMap :Behaviour, IUpdatable 
    {
        public  List<int> Cells { get; }
        private List<int> cells;

        public GenerateMap(GameObject owner, List<int> cells, int columns) : base(owner)
        {
            this.cells = cells;
            Cells      = cells;

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] == 2)
                {
                    Vector2 pos = new Vector2(i % (columns - 1), i / (columns - 1));
                    GenerateTile("Obstacle", pos);
                    //Need to change this one.
                }
                else if (cells[i] == 3)
                {
                    Vector2 pos = new Vector2(i % (columns - 1), i / (columns - 1));
                    GenerateTile("Wall", pos);
                    //i don't know 
                }
           
            }
        }
        public void Update()
        {
            this.cells = Cells;
        }

        private void GenerateTile(string fileName, Vector2 position)
        {
            Tile tile               = new Tile(fileName);
            tile.Transform.Position = position;
            Engine.AddPhysicalObject(tile);
            GameObject t            = Engine.Spawn(tile);
        }
     
    }
}
