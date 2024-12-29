using UnityEngine;
using TMPro;

public class PlayerSkinManager : MonoBehaviour
{
    public GameObject[] playerSkins;
    public string[] skinNames;

    public TextMeshProUGUI skinNameText;

    public GameObject currentSkin;
    int currentSkinIndex = 0;

    public float rotationSpeed = 30f;

    void Start()
    {
        string playerSkin = PlayerPrefs.GetString("PlayerSkin", "dinosaur");

        for (int i = 0; i < playerSkins.Length; i++)
        {
            if (playerSkins[i].name == playerSkin)
            {
                currentSkinIndex = i;
                break;
            }
        }

        InstanciarSkin();
    }

	void Update()
	{
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void PreviousSkin()
    {
        currentSkinIndex = (currentSkinIndex - 1 + playerSkins.Length) % playerSkins.Length;

        InstanciarSkin();
    }

    public void NextSkin()
    {
        currentSkinIndex = (currentSkinIndex + 1) % playerSkins.Length;

        InstanciarSkin();
    }

    void InstanciarSkin()
    {
        if (currentSkin != null)
        {
            Destroy(currentSkin);
        }

        currentSkin = Instantiate(playerSkins[currentSkinIndex], transform);

        if (skinNameText != null)
        {
            if (skinNames[currentSkinIndex] != "")
            {
                skinNameText.text = skinNames[currentSkinIndex];
            }
            else
			{
                skinNameText.text = playerSkins[currentSkinIndex].name;
            }
        }

        PlayerPrefs.SetString("PlayerSkin", playerSkins[currentSkinIndex].name);
        PlayerPrefs.Save();
    }
}
