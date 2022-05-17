using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour
{
    [Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip clip;
    }

    public SoundGroup[] soundGroups;
    private Dictionary<string, AudioClip> _groupDictionary = new ();

    private void Awake()
    {
        foreach (var soundGroup in soundGroups)
        {
            _groupDictionary.Add(soundGroup.groupID, soundGroup.clip);
        }
    }

    public AudioClip GetClipByName(string title)
    {
        return _groupDictionary.ContainsKey(title) ? _groupDictionary[title] : null;
    }
}
