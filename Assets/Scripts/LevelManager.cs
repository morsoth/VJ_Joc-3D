using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int level = -1;

    public void LoadLevel(int lvl)
    {
        level = lvl;
        SceneManager.LoadScene("EndlessLevel");
    }

    public void LoadEndelessLevel()
    {
        level = -1;
        SceneManager.LoadScene("EndlessLevel");
    }
}
