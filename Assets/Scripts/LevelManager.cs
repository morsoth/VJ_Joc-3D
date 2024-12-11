using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadLevel(int level)
    {
        
    }

    public void LoadEndelessLevel()
    {
        SceneManager.LoadScene("EndlessLevel");
    }
}
