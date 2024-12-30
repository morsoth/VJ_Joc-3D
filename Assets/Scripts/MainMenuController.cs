using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public GameObject playerSkinDisplayer;

    public TextMeshProUGUI coinsText;
    int coins;

    void Start()
    {
        LoadCoins();
    }

    void LoadCoins()
	{
        if (PlayerPrefs.HasKey("PlayerCoins"))
        {
            coins = PlayerPrefs.GetInt("PlayerCoins");
        }
        else
        {
            coins = 0;
        }

        if (coinsText != null)
        {
            coinsText.text = "" + coins;
        }
    }
}
