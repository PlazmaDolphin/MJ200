using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewControllerSupport : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollSpeed = 10f;

    private void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null || selected.transform.parent != content) return;

        RectTransform selectedRect = selected.GetComponent<RectTransform>();
        if (selectedRect == null) return;

        // Calculate viewport position of selected element
        RectTransform viewport = scrollRect.viewport;
        Vector3 worldPos = selectedRect.position;
        Vector3 localPos = viewport.InverseTransformPoint(worldPos);

        float halfHeight = viewport.rect.height / 2;

        // If selected is too low > scroll down
        if (localPos.y < -halfHeight * 0.5f)
        {
            float newY = Mathf.Clamp01(scrollRect.verticalNormalizedPosition - Time.deltaTime * scrollSpeed);
            scrollRect.verticalNormalizedPosition = newY;
        }
        // If selected is too high > scroll up
        else if (localPos.y > halfHeight * 0.5f)
        {
            float newY = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + Time.deltaTime * scrollSpeed);
            scrollRect.verticalNormalizedPosition = newY;
        }
    }
}
