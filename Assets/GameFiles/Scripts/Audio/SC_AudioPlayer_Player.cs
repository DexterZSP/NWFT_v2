using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffect
{
    Jump,
    DoubleJump,
    WallJump,
    Attack,
    Dash,
    Death,
    Grapling,
    Hurt,
    Footstep
}

public class SC_AudioPlayer_Player: MonoBehaviour
{

    [System.Serializable]
    public class SoundEffectEntry
    {
        public SoundEffect soundEffect;
        public AudioClip[] audioClips;
    }

    public GameObject audioPlayerPrefab;
    public List<SoundEffectEntry> soundEffectEntries; 

    private Dictionary<SoundEffect, AudioClip[]> soundEffectDictionary;

    private void Awake()
    {
        soundEffectDictionary = new Dictionary<SoundEffect, AudioClip[]>();
        foreach (var entry in soundEffectEntries)
        {
            soundEffectDictionary.Add(entry.soundEffect, entry.audioClips);
        }
    }

    public void PlaySound(SoundEffect soundEffect)
    {
        if (soundEffectDictionary.TryGetValue(soundEffect, out var audioClips))
        {
            AudioClip selectedClip = audioClips[Random.Range(0, audioClips.Length)];

            GameObject audioPlayer = Instantiate(audioPlayerPrefab, transform.position, Quaternion.identity);
            AudioSource audioSource = audioPlayer.GetComponent<AudioSource>();
            audioSource.clip = selectedClip;
            audioSource.Play();

            Destroy(audioPlayer, selectedClip.length);
        }
        else
        {
            Debug.LogWarning($"No se encontró un audio para {soundEffect}");
        }
    }
}

