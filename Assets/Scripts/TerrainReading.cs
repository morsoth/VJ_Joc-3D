using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainReading : MonoBehaviour
{
    public string filePath = "Assets/Levels/Level1";

    [Serializable]
    public class LevelData
    {
        public int rows;
        public int cols;
        public Vector2Int startPoint;
        public int numMaps;
        public List<int[,]> maps = new List<int[,]>();
    }

    public LevelData levelData;

    void Start()
    {
        LoadLevelData(filePath);
    }

    void LoadLevelData(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"El archivo {path} no existe.");
            return;
        }

        string[] lines = File.ReadAllLines(path);
        int currentLine = 0;

        string[] dimensions = ReadNextLine();
        levelData.rows = int.Parse(dimensions[0]);
        levelData.cols = int.Parse(dimensions[1]);

        string[] startPoint = ReadNextLine();
        levelData.startPoint = new Vector2Int(int.Parse(startPoint[0]), int.Parse(startPoint[1]));

        string[] numMaps = ReadNextLine();
        levelData.numMaps = int.Parse(numMaps[0]);


        for (int mapIndex = 0; mapIndex < levelData.numMaps; mapIndex++)
        {
            //currentLine++;

            int[,] map = new int[levelData.rows, levelData.cols];

            for (int row = 0; row < levelData.rows; row++)
            {
                string[] cells = ReadNextLine();
                for (int col = 0; col < levelData.cols; col++)
                {
                    map[row, col] = int.Parse(cells[col]);
                }
            }

            levelData.maps.Add(map);
        }

        string[] ReadNextLine()
        {
            while (string.IsNullOrWhiteSpace(lines[currentLine])) currentLine++;

            return lines[currentLine++].Split(' ');
        }
    }

    void OnDrawGizmos()
    {
        if (levelData != null && levelData.numMaps > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(new Vector3(levelData.startPoint.y, -levelData.startPoint.x, 0), 0.2f);

            Gizmos.color = Color.white;

            for (int mapIndex = 0; mapIndex < levelData.maps.Count; mapIndex++)
            {
                var map = levelData.maps[mapIndex];
                for (int row = 0; row < levelData.rows; row++)
                {
                    for (int col = 0; col < levelData.cols; col++)
                    {
                        if (map[row, col] == 1)
                        {
                            Gizmos.DrawCube(new Vector3(col, -row, 0), Vector3.one * 0.9f);
                        }
                    }
                }
            }
        }
    }
}
