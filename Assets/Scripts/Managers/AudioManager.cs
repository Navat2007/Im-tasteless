using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [SerializeField] private float masterVolumePercent = 1;
    [SerializeField] private float sfxVolumePercent = .6f;
    [SerializeField] private float musicVolumePercent = .4f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource[] musicSources;

    private int _activeMusicSourceIndex;

    private void Awake()
    {
        instance = this;
        
        audioSource.volume = sfxVolumePercent * masterVolumePercent;

        for (int i = 0; i < musicSources.Length; i++)
        {
            musicSources[i].volume = musicVolumePercent * masterVolumePercent;
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position)
    {
        if(clip != null)
            AudioSource.PlayClipAtPoint(clip, position, sfxVolumePercent * masterVolumePercent);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        IEnumerator MusicCrossFade(float duration)
        {
            float percent = 0;

            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / duration;
                musicSources[_activeMusicSourceIndex].volume =
                    Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
                musicSources[1 - _activeMusicSourceIndex].volume =
                    Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);

                yield return null;
            }
        }
        
        _activeMusicSourceIndex = 1 - _activeMusicSourceIndex;
        musicSources[_activeMusicSourceIndex].clip = clip;
        musicSources[_activeMusicSourceIndex].Play();

        StartCoroutine(MusicCrossFade(fadeDuration));
    }
}
