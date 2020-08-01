using UnityEngine;

//------------------------------------------------------\\
//  Manages triggering various reverb types             \\  
//  when the player enters any ReverbArea               \\    
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

[RequireComponent(typeof(AudioReverbZone))]
public class ReverbAreas : MonoBehaviour
{
    private AudioReverbZone reverbZone; //The component used to add various reverb effects

    private void Start()
    {
        reverbZone = GetComponent<AudioReverbZone>();
    }

    public void TriggerReverb(string reverbAreaName)
    {
        Debug.Log("Triggering reverb for area: " + reverbAreaName);

        //Trigger a certain type of reverb based on the passed area name
        //  (i.e. large areas have more reverb than small areas)
        switch (reverbAreaName)
        {
            case "LargeArea":
                reverbZone.reverbPreset = AudioReverbPreset.Stoneroom;
                break;
            case "SmallArea":
                reverbZone.reverbPreset = AudioReverbPreset.PaddedCell;
                break;
            default:
                //Throw an error if an unknown reverbAreaName is passed
                Debug.LogError("Unrecognised name for reverb area: " + reverbAreaName);
                break;
        }
    }
}
