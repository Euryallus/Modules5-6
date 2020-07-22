using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnClick : MonoBehaviour, IPointerClickHandler
{
    public string soundName = "Click";
    public float volume = 1f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AudioManager.instance.PlaySoundEffect2D(soundName, volume);
        }
    }

}
