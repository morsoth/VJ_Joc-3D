using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    int coins;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerCoins"))
        {
            coins = PlayerPrefs.GetInt("PlayerCoins");
        }
        else
        {
            coins = 0;
            PlayerPrefs.SetInt("PlayerCoins", coins);
            PlayerPrefs.Save();
        }

        if (coinsText != null)
        {
            coinsText.text = coins + "  <sprite name=\"coin\">";
        }
    }
}
