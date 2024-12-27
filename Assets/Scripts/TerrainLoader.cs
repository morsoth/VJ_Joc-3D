using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainLoader : MonoBehaviour
{
    public string filePath = "Assets/Levels/Level1";

    [SerializeField] TerrainManager terrainManager;

    Dictionary<Tile, GameObject> tilePrefabMapLight = new Dictionary<Tile, GameObject>();
    Dictionary<Tile, GameObject> tilePrefabMapDark = new Dictionary<Tile, GameObject>();

    [Serializable]
    public class LevelData
    {
        public Vector2Int startPoint;
        public int numMaps;
        public List<int[,]> maps = new List<int[,]>();
    }

    const int ROWS = 8;
    const int COLS = 20;

    public LevelData levelData;

    public GameObject path;

    Tile[,] map = new Tile[ROWS, COLS];
    Height[,] topography = new Height[ROWS, COLS];
    int mapLevel;

    void Start()
    {
        tilePrefabMapLight[Tile.HORIZONTAL] = terrainManager.tilePrefabsLight[0];
        tilePrefabMapLight[Tile.VERTICAL] = terrainManager.tilePrefabsLight[1];
        tilePrefabMapLight[Tile.LEFT_UP] = terrainManager.tilePrefabsLight[2];
        tilePrefabMapLight[Tile.LEFT_DOWN] = terrainManager.tilePrefabsLight[3];
        tilePrefabMapLight[Tile.RIGHT_UP] = terrainManager.tilePrefabsLight[4];
        tilePrefabMapLight[Tile.RIGHT_DOWN] = terrainManager.tilePrefabsLight[5];
        tilePrefabMapLight[Tile.CROSS] = terrainManager.tilePrefabsLight[6];

        tilePrefabMapDark[Tile.HORIZONTAL] = terrainManager.tilePrefabsDark[0];
        tilePrefabMapDark[Tile.VERTICAL] = terrainManager.tilePrefabsDark[1];
        tilePrefabMapDark[Tile.LEFT_UP] = terrainManager.tilePrefabsDark[2];
        tilePrefabMapDark[Tile.LEFT_DOWN] = terrainManager.tilePrefabsDark[3];
        tilePrefabMapDark[Tile.RIGHT_UP] = terrainManager.tilePrefabsDark[4];
        tilePrefabMapDark[Tile.RIGHT_DOWN] = terrainManager.tilePrefabsDark[5];
        tilePrefabMapDark[Tile.CROSS] = terrainManager.tilePrefabsDark[6];

        LoadLevelData(filePath);

        mapLevel = 0;
    }

    public void LoadStage(int stage)
    {
        mapLevel = stage;

        //GenerateTerrain();
    }

    void LoadLevelData(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"El archivo {path} no existe.");
            return;
        }

        levelData.startPoint = new Vector2Int(0, 5);

        string[] lines = File.ReadAllLines(path);
        int currentLine = 0;

        string[] numMaps = ReadNextLine();
        levelData.numMaps = int.Parse(numMaps[0]);


        for (int mapIndex = 0; mapIndex < levelData.numMaps; mapIndex++)
        {
            //currentLine++;

            int[,] map = new int[ROWS, COLS];

            for (int row = 0; row < ROWS; row++)
            {
                string[] cells = ReadNextLine();
                for (int col = 0; col < COLS; col++)
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

    void GenerateTerrain()
	{
        DestroyTerrain();

        //foreach (var (x, y) in pathHistory)
        for (int x = 0; x < COLS; ++x)
        {
            for (int y = 0; y < ROWS; ++y)
            {
                
            }
        }/* 
        
        {
            if (tilePrefabMapLight.ContainsKey(map[y, x]) && tilePrefabMapDark.ContainsKey(map[y, x]))
            {
                float height = (float)topography[y, x] / 2;

                GameObject newTile;
                if ((x + y) % 2 == 0)
                {
                    newTile = Instantiate(
                            tilePrefabMapLight[map[y, x]],
                            new Vector3(y * terrainManager.blockSize.x, (height - (terrainManager.blockSize.y / 4)) * terrainManager.blockSize.y, x * terrainManager.blockSize.z),
                            Quaternion.identity,
                            this.transform
                        );
                }
                else
                {
                    newTile = Instantiate(
                        tilePrefabMapDark[map[y, x]],
                        new Vector3(y * terrainManager.blockSize.x, (height - (terrainManager.blockSize.y / 4)) * terrainManager.blockSize.y, x * terrainManager.blockSize.z),
                        Quaternion.identity,
                        this.transform
                    );
                }

                if (topography[y, x] == Height.DOWN1)
                {
                    GameObject plant = Instantiate(
                        terrainManager.plantPrefab,
                        new Vector3(y * terrainManager.blockSize.x, height * terrainManager.blockSize.y, x * terrainManager.blockSize.z),
                        Quaternion.identity,
                        newTile.transform
                    );
                }
            }
        } */
    }

    void DestroyTerrain()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (levelData != null && levelData.numMaps > 0)
        {
            Gizmos.color = Color.white;

            for (int mapIndex = 0; mapIndex < levelData.maps.Count; mapIndex++)
            {
                var map = levelData.maps[mapIndex];
                for (int row = 0; row < ROWS; row++)
                {
                    for (int col = 0; col < COLS; col++)
                    {
                        if (map[row, col] == 1)
                        {
                            Gizmos.DrawCube(new Vector3(col, -row, mapIndex*5), Vector3.one * 0.9f);
                        }
                    }
                }
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(levelData.startPoint.x, -levelData.startPoint.y, 0), 0.2f);
        }
    }
}
