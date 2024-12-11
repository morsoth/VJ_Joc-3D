using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TerrainManager : MonoBehaviour
{
    [SerializeField] TerrainData terrainData;

    
    Dictionary<Tile, GameObject> tilePrefabMap = new Dictionary<Tile, GameObject>();

    GameObject player;
    GameObject path;

    const int ROWS = 8;
    const int COLS = 20;

    int marginTiles = 4;

    Vector2Int currentTile;

    int numTiles;
    [SerializeField] int maxNumTiles = 35;

    int numObstacles;
    [SerializeField] int minNumObstacles = 2;


    List<(int x, int y)> pathHistory = new List<(int, int)>();

    Tile[,] map = new Tile[ROWS, COLS];
    Height[,] topography = new Height[ROWS, COLS];
    Direction direction;
    Tile prevTile;
    Height prevHeight;

    void Start()
    {
        tilePrefabMap[Tile.HORIZONTAL] = terrainData.tilePrefabs[0];
        tilePrefabMap[Tile.VERTICAL] = terrainData.tilePrefabs[1];
        tilePrefabMap[Tile.LEFT_UP] = terrainData.tilePrefabs[2];
        tilePrefabMap[Tile.LEFT_DOWN] = terrainData.tilePrefabs[3];
        tilePrefabMap[Tile.RIGHT_UP] = terrainData.tilePrefabs[4];
        tilePrefabMap[Tile.RIGHT_DOWN] = terrainData.tilePrefabs[5];
        tilePrefabMap[Tile.CROSS] = terrainData.tilePrefabs[6];

        path = new GameObject("Path");
        path.transform.position = new Vector3(0, 0, 0);

        if (maxNumTiles < COLS) throw new System.Exception("Maximum number of tiles (maxNumTiles) is less than the map width");

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
        DestroyPlayer();

        CreateMap();
        GenerateTopography();
        GenerateTerrain();

        InstantiatePlayer();
    }

    void CreateMap()
	{
        bool isValidMap = false;

        while (!isValidMap)
        {
            try
            {
                ClearPathPoints(); //Cambiar de sitio
                pathHistory.Clear();

                map = new Tile[ROWS, COLS];
                currentTile.x = 0;
                currentTile.y = ROWS - 3;

                numTiles = 0;

                for (int i = 0; i < ROWS; ++i)
                {
                    for (int j = 0; j < COLS; ++j) map[i, j] = Tile.VOID;
                }

                direction = Direction.RIGHT;

                for (int i = 0; i < marginTiles; ++i) PlaceTile(Tile.HORIZONTAL);

                while (IsWithinBounds(currentTile.x, currentTile.y))
                {
                    Tile nextTile = GetRandomTile(prevTile, direction);
                    PlaceTile(nextTile);
                }

                if (currentTile.x != COLS - marginTiles) throw new System.Exception("Path doesn't end correctly");

                for (int i = 0; i < marginTiles; ++i) PlaceTile(Tile.HORIZONTAL);

                CreatePathPoint(currentTile.x-1, currentTile.y);

                isValidMap = true;
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
        if (!terrainData.probabilities.ContainsKey(key)) return Tile.VOID;

        List<(Tile, int)> validTiles = terrainData.probabilities[key].Where(tileChance =>
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
        ++numTiles;
        if (numTiles > maxNumTiles) throw new System.Exception("Limit of tiles exceeded");

        if ((map[currentTile.y, currentTile.x] == Tile.HORIZONTAL && tile == Tile.VERTICAL) || (map[currentTile.y, currentTile.x] == Tile.VERTICAL && tile == Tile.HORIZONTAL))
        {
            map[currentTile.y, currentTile.x] = Tile.CROSS;
        }
        else if (map[currentTile.y, currentTile.x] == Tile.VOID)
        {
            map[currentTile.y, currentTile.x] = tile;
        }

        pathHistory.Add((currentTile.x, currentTile.y));

        if (IsCornerTile(tile)) CreatePathPoint(currentTile.x, currentTile.y);

        prevTile = tile;

        direction = GetDirectionAfterTile(direction, tile);

        switch (direction)
        {
            case Direction.LEFT: currentTile.x--; break;
            case Direction.RIGHT: currentTile.x++; break;
            case Direction.UP: currentTile.y--; break;
            case Direction.DOWN: currentTile.y++; break;
        }
    }

    void CreatePathPoint(int x, int y)
    {
        GameObject point = new GameObject("Point");
        point.transform.SetParent(path.transform);
        point.transform.localPosition = new Vector3(y * terrainData.blockSize.x, 0, x * terrainData.blockSize.z);
    }

    void GenerateTopography()
    {
        bool isValidMap = false;

        while (!isValidMap)
        {
            try
            {
                numObstacles = 0;
                prevHeight = Height.NORMAL;

                foreach (var (x, y) in pathHistory)
                {
                    Height nextHeight = Height.NORMAL;

                    if (map[y, x] == Tile.HORIZONTAL || map[y, x] == Tile.VERTICAL)
                    {
                        if (!IsWithinBounds(x, y)) {
                            nextHeight = Height.NORMAL;
                        }
                        else 
                        {
                            nextHeight = GetNextHeight(prevHeight);
                        }
                    }
                    else if (map[y, x] != Tile.VOID)
                    {
                        nextHeight = Height.NORMAL;
                    }

                    if (nextHeight != Height.NORMAL) ++numObstacles;

                    topography[y, x] = nextHeight;
                    prevHeight = nextHeight;
                }

                if (numObstacles < minNumObstacles) throw new System.Exception("Path has too few obstacles");

                isValidMap = true;
            }
            catch (System.Exception e)
			{
                Debug.Log("Exception: " + e.Message);
            }
        }
    }

    Height GetNextHeight(Height currentHeight)
    {
        List<(Height, int)> validHeights = new List<(Height, int)>(terrainData.heightProbabilities[currentHeight]);

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

                    GameObject newTile = Instantiate(
                            tilePrefabMap[map[i, j]],
                            new Vector3(i * terrainData.blockSize.x, (height - (terrainData.blockSize.y / 4)) * terrainData.blockSize.y, j * terrainData.blockSize.z),
                            Quaternion.identity,
                            this.transform
                        );
                    newTile.gameObject.tag = "Ground";
                }
            }
        }
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
        int x = currentTile.x, y = currentTile.y;

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
        player = Instantiate(terrainData.playerPrefab, new Vector3((ROWS-3) * terrainData.blockSize.x, 0.5f * terrainData.blockSize.y, 0), Quaternion.identity);

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.path = path.transform;
        playerController.terrainManager = this;
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
}
