using UnityEngine;
using System.Collections.Generic;
using System.Linq;

enum Direction { LEFT, RIGHT, UP, DOWN }
enum Tile { VOID, HORIZONTAL, VERTICAL, LEFT_UP, LEFT_DOWN, RIGHT_UP, RIGHT_DOWN, CROSS }
enum Height { VOID = -2, DOWN1 = -1, NORMAL = 0, UP1 = 1, UP2 = 2 }

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

    Dictionary<Height, List<(Height, int)>> heightProbabilities = new Dictionary<Height, List<(Height, int)>>()
    {
        { Height.NORMAL,    new List<(Height, int)> { (Height.NORMAL, 70), (Height.UP1, 18), (Height.UP2, 2), (Height.DOWN1, 10) } },
        { Height.UP1,       new List<(Height, int)> { (Height.NORMAL, 70), (Height.UP1, 20), (Height.DOWN1, 10) } },
        { Height.UP2,       new List<(Height, int)> { (Height.NORMAL, 70), (Height.UP2, 20), (Height.DOWN1, 10) } },
        { Height.DOWN1,     new List<(Height, int)> { (Height.NORMAL, 100) } },
    };

    [SerializeField] GameObject playerPrefab;

    [SerializeField] GameObject[] tilePrefabs;
    Dictionary<Tile, GameObject> tilePrefabMap = new Dictionary<Tile, GameObject>();

    [SerializeField] GameObject player;
    GameObject path;

    const int ROWS = 8;
    const int COLS = 24;

    int currTileX;
    int currTileY;
    int marginTiles;

    List<(int x, int y)> pathHistory = new List<(int, int)>();

    Tile[,] map = new Tile[ROWS, COLS];
    Height[,] topography = new Height[ROWS, COLS];
    Direction direction;
    Tile prevTile;
    Height prevHeight;

    void Start()
    {
        tilePrefabMap[Tile.HORIZONTAL] = tilePrefabs[0];
        tilePrefabMap[Tile.VERTICAL] = tilePrefabs[1];
        tilePrefabMap[Tile.LEFT_UP] = tilePrefabs[2];
        tilePrefabMap[Tile.LEFT_DOWN] = tilePrefabs[3];
        tilePrefabMap[Tile.RIGHT_UP] = tilePrefabs[4];
        tilePrefabMap[Tile.RIGHT_DOWN] = tilePrefabs[5];
        tilePrefabMap[Tile.CROSS] = tilePrefabs[6];

        path = new GameObject("Path");
        path.transform.position = new Vector3(0, 0.25f, 0);

        GenerateLevel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateLevel();
        }
    }

    public void GenerateLevel()
    {
        CreateMap();
        GenerateTopography();
        GenerateTerrain();
        InstantiatePlayer();
    }

    void CreateMap()
	{
        bool isValidMap = false;

        DestroyPlayer();

        while (!isValidMap) {
            try
            {
                ClearPathPoints(); //Cambiar de sitio
                pathHistory.Clear();

                map = new Tile[ROWS, COLS];
                currTileX = 0;
                currTileY = ROWS - 3;
                marginTiles = 5;

                for (int i = 0; i < ROWS; ++i)
                {
                    for (int j = 0; j < COLS; ++j) map[i, j] = Tile.VOID;
                }

                direction = Direction.RIGHT;

                for (int i = 0; i < marginTiles; ++i) PlaceTile(Tile.HORIZONTAL);

                while (IsWithinBounds(currTileX, currTileY))
                {
                    Tile nextTile = GetRandomTile(prevTile, direction);
                    PlaceTile(nextTile);
                }

                if (currTileX != COLS - marginTiles) throw new System.Exception("Path doesn't end correctly");

                isValidMap = true;

                for (int i = 0; i < marginTiles; ++i) PlaceTile(Tile.HORIZONTAL);

                CreatePathPoint(currTileX-1, currTileY);
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
        return Tile.VOID; //ERROR
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

        pathHistory.Add((currTileX, currTileY));

        if (IsCornerTile(tile)) CreatePathPoint(currTileX, currTileY);

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

    void CreatePathPoint(int x, int y)
    {
        GameObject point = new GameObject("Point");
        point.transform.SetParent(path.transform);
        point.transform.localPosition = new Vector3(y, 0, x);
    }

    void GenerateTopography()
    {
        foreach (var (x, y) in pathHistory)
        {
            if (map[y, x] == Tile.HORIZONTAL || map[y, x] == Tile.VERTICAL)
            {
                if (!IsWithinBounds(x, y)) {
                    topography[y, x] = Height.NORMAL;
                    prevHeight = Height.NORMAL;
                }
                else 
                {
                    Height nextHeight = GetNextHeight(prevHeight);
                    topography[y, x] = nextHeight;
                    prevHeight = nextHeight;
                }
            }
            else if (map[y, x] != Tile.VOID)
            {
                topography[y, x] = Height.NORMAL;
                prevHeight = Height.NORMAL;
            }
        }
    }

    Height GetNextHeight(Height currentHeight)
    {
        List<(Height, int)> validHeights = heightProbabilities[currentHeight];
        int totalProbability = validHeights.Sum(p => p.Item2);

        int randomValue = Random.Range(0, totalProbability);
        int cumulative = 0;

        foreach (var (nextHeight, chance) in validHeights)
        {
            cumulative += chance;
            if (randomValue < cumulative)
            {
                return nextHeight;
            }
        }

        return Height.NORMAL;
    }

    void GenerateTerrain()
	{
        DestroyTerrain();

        for (int i = 0; i < ROWS; ++i)
        {
            for (int j = 0; j < COLS; ++j)
            {
                if (map[i, j] != Tile.VOID && tilePrefabMap.ContainsKey(map[i, j]))
                {
                    float height = (float)topography[i, j] / 2;

                    GameObject newTile = Instantiate(tilePrefabMap[map[i, j]], new Vector3(i, height - 0.25f, j), Quaternion.identity, this.transform);
                    newTile.gameObject.tag = "Ground";
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

    void DestroyTerrain()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    void ClearPathPoints()
    {
        for (int i = 0; i < path.transform.childCount; ++i)
        {
            Destroy(path.transform.GetChild(i).gameObject);
        }
    }

    void DestroyPlayer()
    {
        if (player != null) Destroy(player.gameObject);
    }

    bool IsWithinBounds(int x, int y)
    { 
        return (x >= 0 + marginTiles && x < COLS - marginTiles && y >= 0 && y < ROWS);
    }

    bool IsCornerTile(Tile tile)
    {
        return tile == Tile.LEFT_UP || tile == Tile.LEFT_DOWN || tile == Tile.RIGHT_UP || tile == Tile.RIGHT_DOWN;
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
        player = Instantiate(playerPrefab, new Vector3(ROWS-3, 0.25f, 0), Quaternion.identity);

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.path = path.transform;
        playerController.terrainManager = this;
	}
}
