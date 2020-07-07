using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioReverbZone))]
public class ReverbAreas : MonoBehaviour
{
    private AudioReverbZone reverbZone;

    private void Start()
    {
        reverbZone = GetComponent<AudioReverbZone>();
    }

    public void TriggerReverb(string reverbAreaName)
    {
        Debug.Log("Triggering reverb for area: " + reverbAreaName);

        switch (reverbAreaName)
        {
            case "LargeArea":
                reverbZone.reverbPreset = AudioReverbPreset.Stoneroom;
                break;
            case "SmallArea":
                reverbZone.reverbPreset = AudioReverbPreset.PaddedCell;
                break;
            default:
                Debug.LogError("Unrecognised name for reverb area: " + reverbAreaName);
                break;
        }
    }
}
