using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum Direction { LEFT, RIGHT, UP, DOWN }
public enum Tile { VOID, HORIZONTAL, VERTICAL, LEFT_UP, LEFT_DOWN, RIGHT_UP, RIGHT_DOWN, CROSS }
public enum Height { VOID = -3, COIN = -2, DOWN1 = -1, NORMAL = 0, UP1 = 1, UP2 = 2 }

public class TerrainManager : MonoBehaviour
{
    [Header("Level")]
    public int level;

    [Header("Map Settings")]
    public GameObject playerPrefab;

    public GameObject[] tilePrefabsLight;
    public GameObject[] tilePrefabsDark;

    public GameObject plantPrefab;
    public GameObject coinPrefab;

    public Vector3 blockSize;

    [Header("Terrain Objects")]
    [SerializeField] TerrainLoader terrainLoader;
    [SerializeField] TerrainGenerator terrainGenerator;

    GameObject player;

    int map;

    void Start()
    {
        map = 0;

        NextStage();
    }

    public void NextStage()
    {
        DestroyPlayer();

        foreach (Transform child in transform)
        {
            TileController childScript = child.GetComponent<TileController>();
            if (childScript != null)
            {
                childScript.Disapear();
            }
        }

        float delay = 1.0f;

        if (map == 0) delay = 0.0f;
        else delay = 1.0f;

        StartCoroutine(CreateMapWithDelay(delay));

        StartCoroutine(InstantiatePlayerWithDelay(delay+1.0f));
    }

    public void PlayerDie()
	{
        Debug.Log("YOU DIED");
        if (level == 1)
        {
            SceneManager.LoadScene("level01");
        }
        else if (level == 2)
        {
            SceneManager.LoadScene("level02");
        }
        else {
            SceneManager.LoadScene("EndlessLevel");
        }
    }

    public void PlayerWin()
    {
        Debug.Log("YOU WON");
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator CreateMapWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (level == -1)
        {
            terrainGenerator.GenerateStage();
        }
        else
        {
            if (map >= terrainLoader.numMaps)
            {
                PlayerWin();
            }
            else
            {
                terrainLoader.LoadStage(map);
            }
        }

        map++;
    }

    IEnumerator InstantiatePlayerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        InstantiatePlayer();
    }

    void InstantiatePlayer()
	{
        player = Instantiate(playerPrefab, new Vector3((8-3) * blockSize.x, 0.5f * blockSize.y, 0), Quaternion.identity);

        PlayerController playerController = player.GetComponent<PlayerController>();
        if (level == -1)
        {
            playerController.path = terrainGenerator.path.transform;
        }
        else
        {
            playerController.path = terrainLoader.path.transform;
        }
        playerController.terrainManager = this;
	}

    void DestroyPlayer()
    {
        if (player != null) Destroy(player.gameObject);
    }
}
