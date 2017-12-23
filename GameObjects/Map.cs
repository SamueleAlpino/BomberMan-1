using BehaviourEngine;
using BehaviourEngine.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.GameObjects
{
    public class Map : GameObject, IMap
    {
        public static Vector2 PowerUpSpawnPoint { get; private set; }
        private static Vector2 PlayerSpawnPoint { get; set; }
        private static int columnsid;
        private static int[] cellid;

        public static List<Vector2> powerUpSpawnPoints = new List<Vector2>();

        public int[] CellsID => cellid;

        private Node[] mapNodes;
        private GenerateMap renderer;
        private static GenerateMap updater;

        public Map(List<int> cells, int rows, int columns, int index) : base((int)RenderLayer.Background)
        {
            mapNodes = new Node[cells.Count];
            columnsid = columns;
            cellid = cells.ToArray();

            updater  = AddBehaviour<GenerateMap>(new GenerateMap(this, cells, columns));

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i] == 5)
                    PlayerSpawnPoint = new Vector2(i % (columns - 1), i / (columns - 1));

                if (cells[i] == 5 || cells[i] == 0)
                {
                    PowerUpSpawnPoint = new Vector2(i % (columns - 1), i / (columns - 1));
                    powerUpSpawnPoints.Add(PowerUpSpawnPoint);
                }
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < (columns - 1); x++)
                {
                    int indx = y * (columns - 1) + x;

                    if (cells[indx] == 0 || cells[indx] == 5 || cells[indx] == 12)
                    {
                        mapNodes[indx] = new Node(1, new Vector2(x, y));
                    }
                }
            }

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < (columns - 1); x++)
                {
                    int indx = y * (columns - 1) + x;

                    if (mapNodes[indx] == null)
                        continue;

                    // top
                    Node top = GetNodeByIndex(x, y - 1);
                    if (top != null)
                        mapNodes[indx].AddNeighbour(top);

                    // right
                    Node right = GetNodeByIndex(x + 1, y);
                    if (right != null)
                        mapNodes[indx].AddNeighbour(right);

                    // bottom
                    Node bottom = GetNodeByIndex(x, y + 1);
                    if (bottom != null)
                        mapNodes[indx].AddNeighbour(bottom);

                    // left
                    Node left = GetNodeByIndex(x - 1, y);
                    if (left != null)
                        mapNodes[indx].AddNeighbour(left);
                }
            }
        }

        public static bool GetCellMove(int x, int y)
        {
            int index = x + (columnsid - 1) * y;
            if (cellid[index] == 0 || cellid[index] == 5)
                return true;
            return false;
        }

        public static bool GetIndex(bool explosion, int x, int y) // for explosion spawn
        {
            int index = x + (columnsid - 1) * y;
            if (explosion)
            {
                if (cellid[index] == 0 || cellid[index] == 5 || cellid[index] == 12) 
                   return true;

                if (cellid[index] == 3)
                   return false;

                if(cellid[index] == 2)
                {
                   //updater.Cells[index] = 0;
                //    updater.Colliders[index] = null;
                //    updater.Colliders.ToList().Remove(updater.Colliders[index]);
                    return true;
                }
                return false;
            }
            else
            {
                if (cellid[index] == 0)
                    return true;

                return false;
            }
        }

        public static void DestroyBlock(int x, int y)
        {
            int index = x + (columnsid - 1) * y;
            if(!GetIndex(true, x, y))
            {
                cellid[index] = 0;
            }
        }

        public Node GetNodeByIndex(int x, int y) // for node creation
        {

            if (x < 0 || x > 23)
                return null;
            if (y < 0 || y > 11)
                return null;

            int index = y * (columnsid - 1) + x;
            return mapNodes[index];
        }
    }
}
