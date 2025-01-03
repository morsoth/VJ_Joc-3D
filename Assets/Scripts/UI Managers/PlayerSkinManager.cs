using UnityEngine;
using TMPro;

public class PlayerSkinManager : MonoBehaviour
{
    public GameObject[] playerSkins;
    public string[] skinNames;

    public TextMeshProUGUI skinNameText;

    public GameObject currentSkin;
    int currentSkinIndex = 0;

    public float rotationSpeed = 60f;


    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerSkin"))
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
        }

        InstanciarSkin();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            NextSkin();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PreviousSkin();
        }

        UpdateSkin();
    }

	void UpdateSkin()
	{
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void PreviousSkin()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);
        currentSkinIndex = (currentSkinIndex - 1 + playerSkins.Length) % playerSkins.Length;
        InstanciarSkin();
        UpdateSkin();
    }

    public void NextSkin()
    {
        AudioManager.instance.PlaySFX(AudioManager.instance.buttonClickSound);
        currentSkinIndex = (currentSkinIndex + 1) % playerSkins.Length;
        InstanciarSkin();
        UpdateSkin();
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
