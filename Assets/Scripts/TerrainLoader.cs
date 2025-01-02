using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TerrainLoader : MonoBehaviour
{
    [SerializeField] TextAsset levelFile;

    [SerializeField] TerrainManager terrainManager;

    Dictionary<Tile, GameObject> tilePrefabMapLight = new Dictionary<Tile, GameObject>();
    Dictionary<Tile, GameObject> tilePrefabMapDark = new Dictionary<Tile, GameObject>();

    public int numMaps;
    List<Tile[,]> maps = new List<Tile[,]>();
    List<Height[,]> topography = new List<Height[,]>();

    List<(int x, int y)> pathHistory = new List<(int, int)>();

    const int ROWS = 8;
    const int COLS = 20;

    Vector2Int currentTile;
    Direction direction;

    public GameObject path;
    int mapLevel;

    void Awake()
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

        LoadLevelData();

        path = new GameObject("Path");
        path.transform.position = new Vector3(0, 0, 0);

        mapLevel = 0;
    }

    public void LoadStage(int stage)
    {
        mapLevel = stage;

        LoadMap();
        GenerateTerrain();
    }

    void LoadLevelData()
    {
        if (levelFile == null)
        {
            Debug.LogError($"El archivo {levelFile.name} no existe.");
            return;
        }

        string[] lines = levelFile.text.Split('\n');
        int currentLine = 0;

        numMaps = int.Parse(ReadNextLine()[0]);

        for (int m = 0; m < numMaps; m++)
        {
            Tile[,] tileMap = new Tile[ROWS, COLS];
            Height[,] heightMap = new Height[ROWS, COLS];

            for (int row = 0; row < ROWS; row++)
            {
                string[] cells = ReadNextLine();
                for (int col = 0; col < COLS; col++)
                {
                    int cellValue = int.Parse(cells[col]);

                    heightMap[row, col] = cellValue switch
                    {
                        0 => Height.VOID,
                        1 => Height.NORMAL,
                        2 => Height.DOWN1,
                        3 => Height.UP1,
                        4 => Height.UP2,
                        5 => Height.COIN,
                        _ => throw new Exception($"Valor inválido {cellValue} en la posición [{row}, {col}]")
                    };
                }
            }

            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    if (heightMap[row, col] == Height.VOID)
                    {
                        tileMap[row, col] = Tile.VOID;
                        continue;
                    }

                    bool up = row > 0 && heightMap[row - 1, col] != Height.VOID;
                    bool down = row < ROWS - 1 && heightMap[row + 1, col] != Height.VOID;
                    bool left = col > 0 && heightMap[row, col - 1] != Height.VOID;
                    bool right = col < COLS - 1 && heightMap[row, col + 1] != Height.VOID;

                    if (col == 0) left = true;
                    if (col == COLS - 1) right = true;

                    tileMap[row, col] = (up, down, left, right) switch
                    {
                        (true, true, false, false) => Tile.VERTICAL,
                        (false, false, true, true) => Tile.HORIZONTAL,
                        (false, true, true, false) => Tile.LEFT_DOWN,
                        (false, true, false, true) => Tile.RIGHT_DOWN,
                        (true, false, true, false) => Tile.LEFT_UP,
                        (true, false, false, true) => Tile.RIGHT_UP,
                        (true, true, true, true) => Tile.CROSS,
                        _ => Tile.VOID
                    };
                }
            }

            maps.Add(tileMap);
            topography.Add(heightMap);
        }

        string[] ReadNextLine()
        {
            while (string.IsNullOrWhiteSpace(lines[currentLine])) currentLine++;

            return lines[currentLine++].Split(' ');
        }
    }

    void LoadMap()
	{
        ClearPathPoints();
        pathHistory.Clear();

        Tile[,] tileMap = maps[mapLevel];

        currentTile.x = 0;
        currentTile.y = ROWS - 3;

        direction = Direction.RIGHT;

        CreatePathPoint(currentTile.x, currentTile.y);

        while (IsWithinBounds(currentTile.x, currentTile.y))
        {
            Tile tile = tileMap[currentTile.y, currentTile.x];

            if (tile == Tile.VOID) return; //ERROR

            pathHistory.Add((currentTile.x, currentTile.y));

            if (IsCornerTile(tile)) CreatePathPoint(currentTile.x, currentTile.y);

            switch (tile)
            {
                case Tile.LEFT_UP:
                    direction = (direction == Direction.RIGHT) ? Direction.UP : Direction.LEFT;
                    break;
                case Tile.LEFT_DOWN:
                    direction = (direction == Direction.RIGHT) ? Direction.DOWN : Direction.LEFT;
                    break;
                case Tile.RIGHT_UP:
                    direction = (direction == Direction.LEFT) ? Direction.UP : Direction.RIGHT;
                    break;
                case Tile.RIGHT_DOWN:
                    direction = (direction == Direction.LEFT) ? Direction.DOWN : Direction.RIGHT;
                    break;
            }

            switch (direction)
            {
                case Direction.LEFT: currentTile.x--; break;
                case Direction.RIGHT: currentTile.x++; break;
                case Direction.UP: currentTile.y--; break;
                case Direction.DOWN: currentTile.y++; break;
            }
        }

        CreatePathPoint(currentTile.x-1, currentTile.y);
    }

    void GenerateTerrain()
	{
        DestroyTerrain();

        Tile[,] tileMap = maps[mapLevel];
        Height[,] heightMap = topography[mapLevel];

        foreach (var (x, y) in pathHistory)
        {
            if (tileMap[y, x] == Tile.VOID) continue;

            float height = (float)heightMap[y, x] / 2;
            if (heightMap[y, x] == Height.COIN) height = (float)Height.NORMAL / 2;

            Dictionary<Tile, GameObject> tilePrefabMap = (x + y) % 2 == 0 ? tilePrefabMapLight : tilePrefabMapDark;

            if (tilePrefabMap.ContainsKey(tileMap[y, x]))
            {
                GameObject newTile = Instantiate(
                    tilePrefabMap[tileMap[y, x]],
                    new Vector3(y * terrainManager.blockSize.x, (height - (terrainManager.blockSize.y / 4)) * terrainManager.blockSize.y, x * terrainManager.blockSize.z),
                    Quaternion.identity,
                    this.transform
                );

                if (heightMap[y, x] == Height.DOWN1 && terrainManager.plantPrefab != null)
                {
                    Instantiate(
                        terrainManager.plantPrefab,
                        new Vector3(y * terrainManager.blockSize.x, height * terrainManager.blockSize.y, x * terrainManager.blockSize.z),
                        Quaternion.identity,
                        newTile.transform
                    );
                }
                else if (heightMap[y, x] == Height.COIN && terrainManager.coinPrefab != null)
                {
                    Instantiate(
                        terrainManager.coinPrefab,
                        new Vector3(y * terrainManager.blockSize.x, (height + 0.5f) * terrainManager.blockSize.y, x * terrainManager.blockSize.z),
                        Quaternion.identity,
                        newTile.transform
                    );
                }
            }
            else
            {
                Debug.LogWarning($"No se encontró un prefab para el tile {tileMap[y, x]} en la posición [{y}, {x}].");
            }
        }
    }

    void CreatePathPoint(int x, int y)
    {
        GameObject point = new GameObject("Point");
        point.transform.SetParent(path.transform);
        point.transform.localPosition = new Vector3(y * terrainManager.blockSize.x, 0, x * terrainManager.blockSize.z);
    }

    void ClearPathPoints()
    {
        for (int i = 0; i < path.transform.childCount; ++i)
        {
            Destroy(path.transform.GetChild(i).gameObject);
        }
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < COLS && y >= 0 && y < ROWS);
    }

    bool IsCornerTile(Tile tile)
    {
        return tile == Tile.LEFT_UP || tile == Tile.LEFT_DOWN || tile == Tile.RIGHT_UP || tile == Tile.RIGHT_DOWN;
    }

    void DestroyTerrain()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}