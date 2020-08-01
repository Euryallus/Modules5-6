using UnityEngine;
using UnityEngine.EventSystems;

//------------------------------------------------------\\
//  When added to a button, automatically plays the     \\
//  specified sound effect on button press              \\    
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class PlaySoundOnClick : MonoBehaviour, IPointerClickHandler
{
    //Set in inspector:
    public string soundName = "Click";  //Name of the sound to play (as set in AudioManager)
    public float volume = 1f;           //Volume of the sound to play

    public void OnPointerClick(PointerEventData eventData)
    {
        //Play the set sound effect if the left mouse button is clicked
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AudioManager.instance.PlaySoundEffect2D(soundName, volume);
        }
    }

}