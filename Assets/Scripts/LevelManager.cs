using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(int lvl)
    {
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

    public void LoadEndelessLevel()
    {
        SceneManager.LoadScene("EndlessLevel");
    }
}
