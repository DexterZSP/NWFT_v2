using UnityEngine;

public class SC_BGMusic : MonoBehaviour
{
    public static SC_BGMusic Instance { get; private set; }

    [SerializeField] private AudioSource backgroundAudioSource;

    [SerializeField] private AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        backgroundAudioSource = gameObject.GetComponent<AudioSource>();
        backgroundAudioSource.clip = backgroundMusic;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.playOnAwake = false; 
    }

    private void Start()
    {
        if (backgroundMusic != null)
        { PlayBackgroundMusic(); }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundAudioSource != null && backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }
    }

    public void SetBackgroundMusic(AudioClip newMusic)
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Stop();
            backgroundAudioSource.clip = newMusic;
            backgroundAudioSource.Play();
        }
    }
}

