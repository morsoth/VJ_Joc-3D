using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("UI")]
    public TextMeshProUGUI stageText;
    int stage;

    public TextMeshProUGUI percentageText;
    int percentage;

    public TextMeshProUGUI coinsText;
    int coins;

    void Start()
    {
        stage = 0;

        percentage = 0;
        if (percentageText != null)
        {
            percentageText.text = percentage + "%";
        }

        coins = PlayerPrefs.GetInt("PlayerCoins");
        if (coinsText != null)
        {
            coinsText.text = coins + "  <sprite name=\"coin\">";
        }


        NextStage();
    }

	void Update()
	{
		if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Q))
		{
            UpdateProgress();
            SceneManager.LoadScene("MainMenu");
        }
	}

	public void NextStage()
    {
        DestroyPlayer();

        UpdateProgress();

        foreach (Transform child in transform)
        {
            TileController childScript = child.GetComponent<TileController>();
            if (childScript != null)
            {
                childScript.Disapear();
            }
        }

        float delay = 1.0f;

        if (stage == 0) delay = 0.0f;
        else delay = 1.0f;

        StartCoroutine(CreateMapWithDelay(delay));

        StartCoroutine(InstantiatePlayerWithDelay(delay+1.0f));
    }

    public void AddCoin()
    {
        coins++;

        if (coinsText != null)
        {
            coinsText.text = coins + "  <sprite name=\"coin\">";
        }

        PlayerPrefs.SetInt("PlayerCoins", coins);
        PlayerPrefs.Save();
    }

    public void UpdatePercentage(float stagePercentage)
	{
        if (level != -1)
        {
            float decimalPercentage = (stage - 1) * (100f / terrainLoader.numMaps);

            decimalPercentage += (stagePercentage / terrainLoader.numMaps);

            percentage = (int)decimalPercentage;

            if (percentageText != null)
            {
                percentageText.text = percentage + "%";
            }
        }
    }

    void UpdateProgress()
	{
        if (level == 1)
        {
            int maxPercentage = PlayerPrefs.GetInt("ProgressLevel1");
            if (percentage > maxPercentage)
            {
                PlayerPrefs.SetInt("ProgressLevel1", percentage);
                PlayerPrefs.Save();
            }
        }
        else if (level == 2)
        {
            int maxPercentage = PlayerPrefs.GetInt("ProgressLevel2");
            if (percentage > maxPercentage)
            {
                PlayerPrefs.SetInt("ProgressLevel2", percentage);
                PlayerPrefs.Save();
            }
        }
        else
        {
            int maxStages = PlayerPrefs.GetInt("ProgressEndlessLevel");
            if (stage > maxStages)
            {
                PlayerPrefs.SetInt("ProgressEndlessLevel", stage);
                PlayerPrefs.Save();
            }
        }
    }

    public void PlayerDie()
    {
        Debug.Log("YOU DIED");

        UpdateProgress();

        if (level == 1)
        {
            SceneManager.LoadScene("level01");
        }
        else if (level == 2)
        {
            SceneManager.LoadScene("level02");
        }
        else
        {
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
            if (stage >= terrainLoader.numMaps)
            {
                PlayerWin();
            }
            else
            {
                terrainLoader.LoadStage(stage);
            }
        }

        stage++;

        if (stageText != null)
        {
            stageText.text = "Stage: " + stage;
        }
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
