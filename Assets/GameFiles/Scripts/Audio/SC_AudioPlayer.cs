using UnityEngine;

public class SC_AudioPlayer : MonoBehaviour
{
    public static SC_AudioPlayer Instance { get; private set; }

    [SerializeField] private AudioClip[] audioClips;

    [SerializeField] private GameObject audioPrefab;

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
        }
    }

    public void PlaySound(int audioIndex, Vector3 audioPosition)
    {
        if (audioClips == null || audioIndex < 0 || audioIndex >= audioClips.Length)
        {
            Debug.LogError("Índice de audio inválido o lista de clips no asignada.");
            return;
        }

        if (audioPrefab != null)
        {
            GameObject audioObject = Instantiate(audioPrefab, audioPosition, transform.rotation);
            AudioSource audioSource = audioObject.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                audioSource.clip = audioClips[audioIndex];
                audioSource.Play();
                Destroy(audioObject, audioSource.clip.length);
            }
        }
    }
}


