using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public Slider sliderSoundVolume;

    private void Start()
    {
        sliderSoundVolume.value = SaveLoadManager.instance.LoadFloatFromPlayerPrefs("Options_Volume_Sound");
    }

    public void OnVolumeSliderValueChanged(float volume)
    {
        SaveLoadManager.instance.SaveFloatToPlayerPrefs("Options_Volume_Sound", volume);
    }
}
