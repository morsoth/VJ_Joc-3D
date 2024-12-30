using UnityEngine;
using TMPro;

public class PercentageManager : MonoBehaviour
{

    public TextMeshProUGUI percentageText;

    int percentage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("Percentage"))
        {
            percentage = PlayerPrefs.GetInt("Percentage");
        }
        else
        {
            percentage = 0;
        }

        if (percentageText != null)
        {
            percentageText.text = percentage.ToString() + "%";
        }
    }
}
