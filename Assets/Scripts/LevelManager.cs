using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int level = -1;

    public void LoadLevel(int lvl)
    {
        level = lvl;

        switch (lvl)
        {
            case 1:
                SceneManager.LoadScene("Level01");
                break;
            case 2:
                SceneManager.LoadScene("EndlessLevel");
                break;
            default:
                throw new System.Exception("Couldn't find level " + lvl);
        }
    }

    public void LoadEndelessLevel()
    {
        level = -1;
        SceneManager.LoadScene("EndlessLevel");
    }
}
