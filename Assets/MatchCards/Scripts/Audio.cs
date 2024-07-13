using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class Audio : MonoBehaviour
{
    private static Audio instance;
    public static Audio Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<Audio>();
            }

            return instance;
        }
    }
    [SerializeField] public SerializedDictionary<string, AudioClip> SoundsMap = new();
    private static AudioSource Source
    {
        get
        {
            var s = Instance.sources[0];
            Instance.sources.RemoveAt(0);
            Instance.sources.Add(s);
            return s;
        }
    }
    public List<AudioSource> sources = new();
    private bool canPlay = true;

    public static void Play(AudioClip clip)
    {
        if (!Instance.canPlay || clip == null)
        {
            return;
        }
        var source = Source;
        float pitch = 1f;
        source.clip = clip;
        source.pitch = pitch;
        source.Play();
    }

    public static void Play(string name)
    {
        Play(Instance.SoundsMap.GetValueOrDefault(name));
    }
}
