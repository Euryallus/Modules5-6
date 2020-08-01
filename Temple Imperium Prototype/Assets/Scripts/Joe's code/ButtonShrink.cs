using UnityEngine;
using UnityEngine.EventSystems;

//------------------------------------------------------\\
//  ButtonShrink can be applied to any UI button to     \\
//  allow it to grow/shrink when moused over            \\
//------------------------------------------------------\\
//      Written by Joe for prototype phase              \\
//------------------------------------------------------\\

public class ButtonShrink : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Set in inspector, see tooltips for comments:
    [Tooltip("Multiplier that defines how much the button should grow/skrink when hovering")]
    public float hoverSize = 0.95f;
    [Tooltip("How quickly the button should scale towards its target")]
    public float shrinkSpeed = 20f;

    private Vector2 targetScale;    //The scale that the GameObject should lerp towards

    private void Start()
    {
        //The default target scale is (1, 1), i.e. the button's standard size
        targetScale = Vector2.one;
    }

    private void Update()
    {
        //Interpolate towards the targetScale with the set speed, using unscaledDeltaTime so scaling is
        //  framerate independent, and is not affected by Time.timeScale, meaning if the game is paused
        //  the button will still animate at the same speed
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * shrinkSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Set target scale to the value set in the inspector when the mouse enters the button
        targetScale = new Vector2(hoverSize, hoverSize);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Return target scale to default when the mouse leaves the button
        targetScale = Vector2.one;
    }
}
