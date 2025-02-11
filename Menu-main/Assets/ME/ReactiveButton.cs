using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ReactiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = new Vector3(1.1f, 1.1f, 1f);
    public Vector3 clickScale = new Vector3(0.9f, 0.9f, 1f);
    public float animationSpeed = 0.2f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Scale up when hovered
        LeanTween.scale(gameObject, hoverScale, animationSpeed).setEase(LeanTweenType.easeOutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Return to normal size
        LeanTween.scale(gameObject, normalScale, animationSpeed).setEase(LeanTweenType.easeOutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Shrink when clicked
        LeanTween.scale(gameObject, clickScale, animationSpeed).setEase(LeanTweenType.easeOutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Return to hover size after clicking
        LeanTween.scale(gameObject, hoverScale, animationSpeed).setEase(LeanTweenType.easeOutQuad);
    }
}
