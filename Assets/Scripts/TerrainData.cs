using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Direction { LEFT, RIGHT, UP, DOWN }
public enum Tile { VOID, HORIZONTAL, VERTICAL, LEFT_UP, LEFT_DOWN, RIGHT_UP, RIGHT_DOWN, CROSS }
public enum Height { VOID = -2, DOWN1 = -1, NORMAL = 0, UP1 = 1, UP2 = 2 }

public class TerrainData : MonoBehaviour
{
    public Dictionary<(Tile, Direction), List<(Tile, int)>> probabilities = new Dictionary<(Tile, Direction), List<(Tile, int)>>()
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

    public Dictionary<Height, List<(Height, int)>> heightProbabilities = new Dictionary<Height, List<(Height, int)>>()
    {
        { Height.NORMAL,    new List<(Height, int)> { (Height.NORMAL, 70), (Height.UP1, 18), (Height.UP2, 2), (Height.DOWN1, 10) } },
        { Height.UP1,       new List<(Height, int)> { (Height.NORMAL, 70), (Height.UP1, 20), (Height.DOWN1, 10) } },
        { Height.UP2,       new List<(Height, int)> { (Height.NORMAL, 70), (Height.UP2, 20), (Height.DOWN1, 10) } },
        { Height.DOWN1,     new List<(Height, int)> { (Height.NORMAL, 100) } },
    };

    public GameObject playerPrefab;

    public GameObject[] tilePrefabs;

    public Vector3 blockSize;
}
