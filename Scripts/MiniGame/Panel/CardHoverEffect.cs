using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;

    private Vector3 targetScale;
    private bool isHovered;

    private Button button;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        // 같은 오브젝트에 있는 Button 컴포넌트 가져오기
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && button.interactable)
        {
            isHovered = true;
            targetScale = originalScale * hoverScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }
}
