using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonShrink : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverSize = 0.95f;
    public float shrinkSpeed = 20f;
    private Vector2 targetScale;

    private void Start()
    {
        targetScale = Vector2.one;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * shrinkSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = new Vector2(hoverSize, hoverSize);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector2.one;
    }
}
