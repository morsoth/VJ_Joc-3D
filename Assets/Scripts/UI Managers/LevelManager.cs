using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            LoadLevel(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LoadLevel(2);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            LoadEndlessLevel();
        }
    }

    public void LoadLevel(int lvl)
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);

        switch (lvl)
        {
            case 1:
                SceneManager.LoadScene("Level01");
                break;
            case 2:
                SceneManager.LoadScene("Level02");
                break;
            default:
                throw new System.Exception("Couldn't find level " + lvl);
        }
    }

    public void LoadEndlessLevel()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);

        SceneManager.LoadScene("EndlessLevel");
    }
}
