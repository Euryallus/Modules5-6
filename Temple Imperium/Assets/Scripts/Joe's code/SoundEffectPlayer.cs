using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    public static SoundEffectPlayer instance;

    [Header("Add sound effects:")]
    [Header("Please make changes on the prefab object.")]

    //Set in inspector:
    [SerializeField]
    private SoundEffect[] soundEffects;
    [SerializeField]
    private GameObject prefabSoundSource;

    private Dictionary<string, AudioClip> soundEffectsDict;
    private bool ready = false;

    private void Awake()
    {
        //Ensure that an instance of the SoundEffectPlayer class does not already exist
        if (instance == null)
        {
            //Set this class as the instance and ensure that it stays when changing scenes
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //If there is an existing instance that is not this, destroy the GameObject this script is connected to
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupSoundEffectsDict();
        ready = true;

        StartCoroutine(SoundSourceCleanupCoroutine());
    }

    public void SetupSoundEffectsDict()
    {
        soundEffectsDict = new Dictionary<string, AudioClip>();
        for (int i = 0; i < soundEffects.Length; i++)
        {
            soundEffectsDict.Add(soundEffects[i].name, soundEffects[i].audioClip);
        }
    }

    /// <summary> Plays a sound effect with the given name and parameters. </summary>
    /// <param name="name">The name given to the target sound in the inspector.</param>
    /// <param name="use3dSpace">Should this sound be treated as a 3D sound? Should be true for atmospheric sounds, false for UI.</param>
    /// <param name="sourcePosition">If using 3D space, the sound will originate from this position. For 2D sounds, this can be set to any value.</param>
    /// <param name="volume">The volume of the sound to be played.</param>
    /// <param name="minPitch">The lowest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    /// <param name="maxPitch">The highest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    public void PlayGenericSoundEffect(string name, bool use3dSpace, Vector3 sourcePosition, float volume = 1f, float minPitch = 1f, float maxPitch = 1f, bool looping = false, string loopId = "")
    {
        if (!ready)
        {
            Debug.LogWarning("Trying to play sound effect: " + name + " - not ready.");
        }

        if (!soundEffectsDict.ContainsKey(name))
        {
            Debug.LogError("Trying to play sound with invalid name: " + name);
            return;
        }

        //Ensure maxPitch is not lower than minPitch
        maxPitch = Mathf.Clamp(maxPitch, minPitch, Mathf.Infinity);

        GameObject goSource = Instantiate(prefabSoundSource, sourcePosition, Quaternion.identity, transform);
        AudioSource audioSource = goSource.GetComponent<AudioSource>();
        audioSource.clip = soundEffectsDict[name];
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioSource.volume = volume;
        if (use3dSpace)
        {
            audioSource.spatialBlend = 1f;
        }
        audioSource.Play();
    }
    public void PlayStandardSoundEffect(string name, Vector3 sourcePosition, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        PlayGenericSoundEffect(name, true, sourcePosition, volume, minPitch, maxPitch);
    }
    public void PlayUISoundEffect(string name, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        PlayGenericSoundEffect(name, false, Vector3.zero, volume, minPitch, maxPitch);
    }
    public void PlayLoopingSoundEffect(string name, Vector3 sourcePosition, string loopId, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        PlayGenericSoundEffect(name, true, sourcePosition, volume, minPitch, maxPitch, true, loopId);
    }

    private IEnumerator SoundSourceCleanupCoroutine()
    {
        //Wait for 10 frames between each cleanup
        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }

        //Remove unused audio sources
        CleanupSoundEffectSources();

        //Repeat
        StartCoroutine(SoundSourceCleanupCoroutine());
    }
    private void CleanupSoundEffectSources()
    {
        foreach (Transform child in transform)
        {
            if (!child.GetComponent<AudioSource>().isPlaying)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

[Serializable]
public struct SoundEffect
{
    public string name;
    public AudioClip audioClip;
}

