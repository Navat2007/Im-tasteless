using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    
    [SerializeField] private AudioClip[] musicClips;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (musicClips.Length > 0)
        {
            System.Random random = new System.Random();
            AudioManager.instance.PlayMusic(musicClips[random.Next(musicClips.Length)], 2f);
        }
            
    }
}
