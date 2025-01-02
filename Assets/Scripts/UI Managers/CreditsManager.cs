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

    public void ShowCredits()
    {
        panel.SetActive(true);
    }

    public void HideCredits()
    {
        panel.SetActive(false);
    }
}
