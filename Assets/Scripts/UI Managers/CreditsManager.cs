using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CreditsManager : MonoBehaviour
{
    public GameObject panel;

	void Start()
	{
		panel.SetActive(false);
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShowCredits();
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            HideCredits();
            AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);
        }
        
    }

    public void ShowCredits()
    {
        panel.SetActive(true);
    }

    public void HideCredits()
    {
        panel.SetActive(false);
    }
}
