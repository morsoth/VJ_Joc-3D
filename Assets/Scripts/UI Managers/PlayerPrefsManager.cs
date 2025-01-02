using UnityEngine;

public class ResetPlayerPrefs : MonoBehaviour
{
	public bool resetPlayerPrefs = false;

	void Update()
	{
		if (resetPlayerPrefs)
		{
			resetPlayerPrefs = false;
			PlayerPrefs.DeleteAll();
		}
	}
}
