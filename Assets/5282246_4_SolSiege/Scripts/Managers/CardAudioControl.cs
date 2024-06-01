using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class CardAudioControl : Singleton<CardAudioControl>
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip clipOneCard;
    [SerializeField] private AudioClip clipShuffleCard;

    public void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        GameManager.OnSoundVolumeChanged += SetVolume;
        SetVolume(GameManager.soundVolume);
    }

    public void PlayOneCard() {
        if (clipOneCard != null)
        {
            audioSource.PlayOneShot(clipOneCard);
        }
    }

    public void PlayShuffle() {
        if (clipShuffleCard != null)
        {
            audioSource.PlayOneShot(clipShuffleCard);
        }
    }

    private void SetVolume(float volume) {
        audioSource.volume = volume;
    }

    public override void OnDestroy()
    {
        GameManager.OnSoundVolumeChanged -= SetVolume;
        base.OnDestroy();
    }
}
