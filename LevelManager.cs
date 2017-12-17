using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BehaviourEngine;
using System.IO;
using BomberMan.GameObjects;

namespace BomberMan
{
    public class Level
    {
        private static Dictionary<string, Level> instances;
        private List<int> map;
        private int rows;
        private int columns;
        private int index;
        private static GameObject gameObj;
        public Map currentMap;

        static Level()
        {
            instances = new Dictionary<string, Level>();
        }

        public Level(string fileName, string levelName, int index)
        {
            map = new List<int>();
            this.index = index;
            ReadFromFile(fileName);
            instances.Add(levelName, this);
        }

        private void ReadFromFile(string csvFileName)
        {
            string[] lines = File.ReadAllLines(csvFileName);
            this.rows = lines.Length;

            foreach (string t1 in lines)
            {
                string[] values = t1.Trim().Split(',');
                if (this.columns == 0)
                    this.columns = values.Length;

                foreach (string t in values)
                {
                    int value;
                    string currentVal = t.Trim();
                    bool success = int.TryParse(currentVal, out value);
                    if (success)
                        map.Add(value);
                }
            }
        }

        private void LoadMap()
        {
            currentMap = new Map(map, rows, columns, index);
            gameObj = Engine.Spawn(currentMap);
        }

        public void NextLevel(bool next)
        {
            Engine.Destroy(gameObj); // questo fa schifo
            if (next)
                index++;
            else
                index--;
            string level = "Base" + index;
            Get(level).LoadMap();
        }

        public static Level Get(string levelName)
        {
            return instances[levelName];
        }

        public static void Load(string levelName)
        {
            try
            {
                instances[levelName].LoadMap();
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
