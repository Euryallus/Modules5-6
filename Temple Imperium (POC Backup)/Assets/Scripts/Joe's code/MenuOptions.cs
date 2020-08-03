using UnityEngine;
using UnityEngine.UI;

//------------------------------------------------------\\
//  Used for setting options in the main menu           \\
//------------------------------------------------------\\
//      Written by Joe for proof of concept phase       \\
//------------------------------------------------------\\

public class MenuOptions : MonoBehaviour
{
    //Set in inspector:
    public Slider sliderSoundVolume;    //UI slider used for showing/adjusting sound volume

    private void Start()
    {
        //Set the slider to the saved sound volume by default
        sliderSoundVolume.value = SaveLoadManager.instance.LoadFloatFromPlayerPrefs("Options_Volume_Sound");
    }

    public void OnVolumeSliderValueChanged(float volume)
    {
        //When the slider is adjusted, save the new volume based on the slider value
        SaveLoadManager.instance.SaveFloatToPlayerPrefs("Options_Volume_Sound", volume);
    }
}
