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
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            HideCredits();
        }
        
    }

    public void ShowCredits()
    {
        panel.SetActive(true);
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);
    }

    public void HideCredits()
    {
        panel.SetActive(false);
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);
    }
}
