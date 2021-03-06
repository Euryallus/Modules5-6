﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//------------------------------------------------------\\
//  SaveLoadManager provides an easy interface for      \\
//  sounds of various types to be played from any scene \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class SoundEffectPlayer : MonoBehaviour
{
    public static SoundEffectPlayer instance;

    [Header("Add sound effects:")]
    [Header("Please make changes on the prefab object.")]
    //Set in inspector:
    [SerializeField]
    private SoundEffect[] soundEffects;     //All available sound effects
    [SerializeField]
    private GameObject prefabSoundSource;   //Sound source to be instantiated when a sound is played

    private Dictionary<string, AudioClip> soundEffectsDict; //Dictionary so audio clips can be accessed based on their name

    public SoundEffect[] GetSoundEffects() { return soundEffects; }

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

    void Start()
    {
        SetupSoundEffectsDict();
        //Start the sound effect cleanup coroutine that will loop for the duration of the game
        StartCoroutine(SoundSourceCleanupCoroutine());
    }

    public void SetupSoundEffectsDict()
    {
        //Add all sounds to the sound effects dictionary, using their name as a key
        soundEffectsDict = new Dictionary<string, AudioClip>();
        for (int i = 0; i < soundEffects.Length; i++)
        {
            soundEffectsDict.Add(soundEffects[i].name, soundEffects[i].audioClip);
        }
    }

    #region Playing/Stopping Sounds

    public void PlayGenericSoundEffect(string name, bool use3dSpace, Vector3 sourcePosition, float volume = 1f, float minPitch = 1f, float maxPitch = 1f, bool looping = false, string loopId = "")
    {
        //Check that the specified sound exists and throw an error if now
        if (!soundEffectsDict.ContainsKey(name))
        {
            Debug.LogError("Trying to play sound with invalid name: " + name);
            return;
        }

        //Ensure maxPitch is not lower than minPitch
        maxPitch = Mathf.Clamp(maxPitch, minPitch, Mathf.Infinity);

        //Create the sound source GameObject
        GameObject goSource = Instantiate(prefabSoundSource, sourcePosition, Quaternion.identity, transform);
        goSource.name = "Sound_" + name;

        //Set audioSource values based on given parameters
        AudioSource audioSource = goSource.GetComponent<AudioSource>();
        audioSource.clip = soundEffectsDict[name];
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        //Multiply volume by the value set in the options menu
        audioSource.volume = volume * SaveLoadManager.instance.LoadFloatFromPlayerPrefs("Options_Volume_Sound");

        if (use3dSpace)
        {
            //Enable spatialBlend if playing sound in 3D space, so it will sound like it originates from sourcePosition
            audioSource.spatialBlend = 1f;
        }
        if (looping)
        {
            //If looping, set the audioSource to loop and give it an identifiable name so it can later be stopped/deleted
            goSource.name = "LoopSound_" + loopId;
            audioSource.loop = true;
        }
        //Play the sound
        audioSource.Play();
    }

    /// <summary> Plays a sound effect in 3D space with the given name and parameters. </summary>
    /// <param name="name">The name given to the target sound in the inspector.</param>
    /// <param name="sourcePosition">The position in the scene that the sound will originate from.</param>
    /// <param name="volume">The volume of the sound to be played.</param>
    /// <param name="minPitch">The lowest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    /// <param name="maxPitch">The highest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    public void PlaySoundEffect3D(string name, Vector3 sourcePosition, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        PlayGenericSoundEffect(name, true, sourcePosition, volume, minPitch, maxPitch);
    }

    /// <summary> Plays a sound effect using a 2D source with the given name and parameters. </summary>
    /// <param name="name">The name given to the target sound in the inspector.</param>
    /// <param name="volume">The volume of the sound to be played.</param>
    /// <param name="minPitch">The lowest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    /// <param name="maxPitch">The highest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    public void PlaySoundEffect2D(string name, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        PlayGenericSoundEffect(name, false, Vector3.zero, volume, minPitch, maxPitch);
    }

    /// <summary> Plays a sound effect that will loop until StopLoopingSoundEffect is called with the given loopId. </summary>
    /// <param name="name">The name given to the target sound in the inspector.</param>
    /// <param name="use3dSpace">Changes whether this sound will use a 2D or 3D audio source.</param>
    /// <param name="sourcePosition">The position in the scene that the sound will originate from. Can be set to any value for 2D sounds.</param>
    /// <param name="loopId">A unique id given to this sound that can be used to stop it from playing.</param>
    /// <param name="volume">The volume of the sound to be played.</param>
    /// <param name="minPitch">The lowest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    /// <param name="maxPitch">The highest pitch that can be used when playing the sound. A random value between minPitch and maxPitch will be chosen.</param>
    public void PlayLoopingSoundEffect(string name, bool use3dSpace, Vector3 sourcePosition, string loopId, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        PlayGenericSoundEffect(name, use3dSpace, sourcePosition, volume, minPitch, maxPitch, true, loopId);
    }

    /// <summary> Stops a looping sound with loopId from playing. </summary>
    /// <param name="loopId">A unique id that was given to the target sound when PlayLoopingSoundEffect was called.</param>
    public void StopLoopingSoundEffect(string loopId)
    {
        GameObject goLoopSource = GameObject.Find("LoopSound_" + loopId);
        if(goLoopSource != null)
        {
            Destroy(goLoopSource);
        }
    }

    #endregion

    //Used to remove inactive sound sources every few frames
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
        //Find and destroy any sources that are not playing
        foreach (Transform child in transform)
        {
            if (!child.GetComponent<AudioSource>().isPlaying)
            {
                Destroy(child.gameObject);
            }
        }
    }
}

//Used to define sound effects in the editor, each SoundEffect has
//  - a name used to identify it
//  - an audioClip, the actual sound to be played
[Serializable]
public struct SoundEffect
{
    public string name;
    public AudioClip audioClip;
}