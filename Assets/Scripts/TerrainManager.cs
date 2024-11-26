using UnityEngine;
using System.Collections.Generic;
using System.Linq;

enum Direction { LEFT, RIGHT, UP, DOWN }
enum Tile { VOID, HORIZONTAL, VERTICAL, LEFT_UP, LEFT_DOWN, RIGHT_UP, RIGHT_DOWN, CROSS }

public class TerrainManager : MonoBehaviour
{
    Dictionary<(Tile, Direction), List<(Tile, int)>> probabilities = new Dictionary<(Tile, Direction), List<(Tile, int)>>()
    {
        { (Tile.HORIZONTAL, Direction.LEFT),    new List<(Tile, int)>{ (Tile.HORIZONTAL, 30), (Tile.RIGHT_UP, 35), (Tile.RIGHT_DOWN, 35) } },
        { (Tile.HORIZONTAL, Direction.RIGHT),   new List<(Tile, int)>{ (Tile.HORIZONTAL, 70), (Tile.LEFT_UP, 15), (Tile.LEFT_DOWN, 15) } },
        { (Tile.VERTICAL, Direction.UP),        new List<(Tile, int)>{ (Tile.VERTICAL, 40), (Tile.LEFT_DOWN, 20), (Tile.RIGHT_DOWN, 40) } },
        { (Tile.VERTICAL, Direction.DOWN),      new List<(Tile, int)>{ (Tile.VERTICAL, 40), (Tile.LEFT_UP, 20), (Tile.RIGHT_UP, 40) } },
        { (Tile.LEFT_UP, Direction.LEFT),       new List<(Tile, int)>{ (Tile.HORIZONTAL, 100) } },
        { (Tile.LEFT_UP, Direction.UP),         new List<(Tile, int)>{ (Tile.VERTICAL, 100) } },
        { (Tile.LEFT_DOWN, Direction.LEFT),     new List<(Tile, int)>{ (Tile.HORIZONTAL, 100) } },
        { (Tile.LEFT_DOWN, Direction.DOWN),     new List<(Tile, int)>{ (Tile.VERTICAL, 100) } },
        { (Tile.RIGHT_UP, Direction.RIGHT),     new List<(Tile, int)>{ (Tile.HORIZONTAL, 100) } },
        { (Tile.RIGHT_UP, Direction.UP),        new List<(Tile, int)>{ (Tile.VERTICAL, 100) } },
        { (Tile.RIGHT_DOWN, Direction.RIGHT),   new List<(Tile, int)>{ (Tile.HORIZONTAL, 100) } },
        { (Tile.RIGHT_DOWN, Direction.DOWN),    new List<(Tile, int)>{ (Tile.VERTICAL, 100) } },
        { (Tile.CROSS, Direction.LEFT),         new List<(Tile, int)>{ (Tile.HORIZONTAL, 100) } },
        { (Tile.CROSS, Direction.RIGHT),        new List<(Tile, int)>{ (Tile.HORIZONTAL, 100) } },
        { (Tile.CROSS, Direction.UP),           new List<(Tile, int)>{ (Tile.VERTICAL, 100) } },
        { (Tile.CROSS, Direction.DOWN),         new List<(Tile, int)>{ (Tile.VERTICAL, 100) } },
    };

    [SerializeField] GameObject[] tilePrefabs;
    Dictionary<Tile, GameObject> tilePrefabMap = new Dictionary<Tile, GameObject>();

    const int ROWS = 8;
    const int COLS = 15;

    int currTileX;
    int currTileY;

    Tile[,] map = new Tile[ROWS, COLS];
    Direction direction;
    Tile prevTile;

    void Start()
    {
        tilePrefabMap[Tile.HORIZONTAL] = tilePrefabs[0];
        tilePrefabMap[Tile.VERTICAL] = tilePrefabs[1];
        tilePrefabMap[Tile.LEFT_UP] = tilePrefabs[2];
        tilePrefabMap[Tile.LEFT_DOWN] = tilePrefabs[3];
        tilePrefabMap[Tile.RIGHT_UP] = tilePrefabs[4];
        tilePrefabMap[Tile.RIGHT_DOWN] = tilePrefabs[5];
        tilePrefabMap[Tile.CROSS] = tilePrefabs[6];

        CreateMap();
        InstantiatePlayer();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateMap();
            InstantiatePlayer();
        }
    }

    void CreateMap()
	{
        bool isValidMap = false;

        while (!isValidMap) {
            try
            {
                map = new Tile[ROWS, COLS];
                currTileX = 0;
                currTileY = ROWS - 3;

                for (int i = 0; i < ROWS; ++i)
                {
                    for (int j = 0; j < COLS; ++j) map[i, j] = Tile.VOID;
                }

                direction = Direction.RIGHT;
                map[currTileY, currTileX] = Tile.HORIZONTAL;
                prevTile = Tile.HORIZONTAL;
                currTileX++;

                while (IsWithinBounds(currTileX, currTileY))
                {
                    Tile nextTile = GetRandomTile(prevTile, direction);
                    PlaceTile(nextTile);
                }

                if (currTileX != COLS) throw new System.Exception("Path doesn't end correctly");

                isValidMap = true;

                GenerateTerrain();
            }
            catch (System.Exception e)
			{
                Debug.Log("Exception: " + e.Message);
            }
        }
    }

    Tile GetRandomTile(Tile currentTile, Direction direction)
    {
        var key = (currentTile, direction);
        if (!probabilities.ContainsKey(key)) return Tile.VOID;

        List<(Tile, int)> validTiles = probabilities[key].Where(tileChance =>
        {
            Tile tile = tileChance.Item1;
            Direction testDirection = GetDirectionAfterTile(direction, tile);
            return CanPlaceTile(testDirection);
        }).ToList();

        if (validTiles.Count == 0) throw new System.Exception("No valid tiles available");

        int totalProbability = validTiles.Sum(tileChance => tileChance.Item2);

        int random = Random.Range(0, totalProbability);
        int cumulative = 0;

        foreach (var (tile, chance) in validTiles)
        {
            cumulative += chance;
            if (random < cumulative) return tile;
        }
        return Tile.VOID;
    }

    Direction GetDirectionAfterTile(Direction currentDirection, Tile tile)
    {
        switch (tile)
        {
            case Tile.LEFT_UP:
                return currentDirection == Direction.RIGHT ? Direction.UP : Direction.LEFT;
            case Tile.LEFT_DOWN:
                return currentDirection == Direction.RIGHT ? Direction.DOWN : Direction.LEFT;
            case Tile.RIGHT_UP:
                return currentDirection == Direction.LEFT ? Direction.UP : Direction.RIGHT;
            case Tile.RIGHT_DOWN:
                return currentDirection == Direction.LEFT ? Direction.DOWN : Direction.RIGHT;
            default:
                return currentDirection;
        }
    }

    void PlaceTile(Tile tile)
	{
        if ((map[currTileY, currTileX] == Tile.HORIZONTAL && tile == Tile.VERTICAL) || (map[currTileY, currTileX] == Tile.VERTICAL && tile == Tile.HORIZONTAL))
        {
            map[currTileY, currTileX] = Tile.CROSS;
        }
        else if (map[currTileY, currTileX] == Tile.VOID)
        {
            map[currTileY, currTileX] = tile;
        }

        prevTile = tile;

        direction = GetDirectionAfterTile(direction, tile);

        switch (direction)
        {
            case Direction.LEFT: currTileX--; break;
            case Direction.RIGHT: currTileX++; break;
            case Direction.UP: currTileY--; break;
            case Direction.DOWN: currTileY++; break;
        }
    }

    void GenerateTerrain()
	{
        DestroyChildren();

        for (int i = 0; i < ROWS; ++i)
        {
            for (int j = 0; j < COLS; ++j)
            {
                if (map[i, j] != Tile.VOID && tilePrefabMap.ContainsKey(map[i, j]))
                {
                    Instantiate(tilePrefabMap[map[i, j]], new Vector3(i, 0, j), Quaternion.identity, this.transform);
                }
            }
        }
    }

    void PrintMap()
	{
        for (int i = 0; i < ROWS; ++i)
        {
            string fila = "";
            for (int j = 0; j < COLS; ++j)
            {
                fila += (int)map[i, j] + "\t";
            }
            Debug.Log(fila);
        }
        Debug.Log("------------------------------------------------------");
    }

    void DestroyChildren()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    bool IsWithinBounds(int x, int y)
    { 
        return (x >= 0 && x < COLS && y >= 0 && y < ROWS);
    }

    bool CanPlaceTile(Direction direction)
    {
        int x = currTileX, y = currTileY;

        switch (direction)
        {
            case Direction.LEFT: x--; break;
            case Direction.RIGHT: x++; break;
            case Direction.UP: y--; break;
            case Direction.DOWN: y++; break;
        }

        if (!IsWithinBounds(x, y)) return true;

        if ((direction == Direction.LEFT || direction == Direction.RIGHT) && map[y, x] == Tile.VERTICAL)
        {
            return true;
        }
        else if ((direction == Direction.UP || direction == Direction.DOWN) && map[y, x] == Tile.HORIZONTAL)
        {
            return true;
        }

        return map[y, x] == Tile.VOID;
    }

    void InstantiatePlayer()
	{
        // Instantiate
        // Pass points
        // Activate movement
	}
}
