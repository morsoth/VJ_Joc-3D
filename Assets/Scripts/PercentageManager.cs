using UnityEngine;
using TMPro;

public class PercentageManager : MonoBehaviour
{

    public TextMeshProUGUI percentageTextLevel1;
    public TextMeshProUGUI percentageTextLevel2;
    public TextMeshProUGUI stagesTextEndlessLevel;

    void Start()
    {
        LoadProgressLevel1();
        LoadProgressLevel2();
        LoadProgressEndlessLevel();
    }

    void LoadProgressLevel1()
	{
        int progress;

        if (PlayerPrefs.HasKey("ProgressLevel1"))
        {
            progress = PlayerPrefs.GetInt("ProgressLevel1");
        }
        else
        {
            progress = 0;
            PlayerPrefs.SetInt("ProgressLevel1", progress);
            PlayerPrefs.Save();
        }

        if (percentageTextLevel1 != null)
        {
            percentageTextLevel1.text = progress.ToString() + "%";
        }
    }

    void LoadProgressLevel2()
    {
        int progress;

        if (PlayerPrefs.HasKey("ProgressLevel2"))
        {
            progress = PlayerPrefs.GetInt("ProgressLevel2");
        }
        else
        {
            progress = 0;
            PlayerPrefs.SetInt("ProgressLevel2", progress);
            PlayerPrefs.Save();
        }

        if (percentageTextLevel2 != null)
        {
            percentageTextLevel2.text = progress.ToString() + "%";
        }
    }

    void LoadProgressEndlessLevel()
    {
        int progress;

        if (PlayerPrefs.HasKey("ProgressEndlessLevel"))
        {
            progress = PlayerPrefs.GetInt("ProgressEndlessLevel");
        }
        else
        {
            progress = 0;
            PlayerPrefs.SetInt("ProgressEndlessLevel", progress);
            PlayerPrefs.Save();
        }

        if (stagesTextEndlessLevel != null)
        {
            stagesTextEndlessLevel.text = progress.ToString() + " stages";
        }
    }
}
