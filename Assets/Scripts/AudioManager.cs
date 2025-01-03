using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    public AudioSource sfxSource;
    [Range(0f, 1f)] public float sfxVolume = 0.5f;

    [Header("Audio Clips")]
    public AudioClip musicTrack;

    public AudioClip buttonClickSound;

    public AudioClip coinSound;
    public AudioClip jumpSound;
    public AudioClip fallSound;
    public AudioClip deathSound;
    public AudioClip terrainSound;
    public AudioClip winSound;

    public static AudioManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

	private void Start()
	{
        PlayMusic();
    }

	void Update()
	{
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
            musicSource.loop = true;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

	public void PlayMusic()
    {
        if (musicSource != null && musicTrack != null)
        {
            musicSource.clip = musicTrack;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(AudioClip audio)
    {
        if (sfxSource != null && audio != null)
        {
            sfxSource.PlayOneShot(audio);
        }
    }
}
