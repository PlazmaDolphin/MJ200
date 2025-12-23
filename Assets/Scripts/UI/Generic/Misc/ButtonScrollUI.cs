using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScrollUI : MonoBehaviour
{
    private MenuEventSystemHandler menuEventSystemHandler;

    [Header("Rects")]
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform focusTarget;

    [Space(20f), Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    private Vector2 targetPosition;

    private Selectable lastSelected;
    private List<Selectable> trackedButtons;

    void Awake()
    {
        Debug.Log("Make button scroll UI work");

        menuEventSystemHandler = GetComponent<MenuEventSystemHandler>();

        trackedButtons = menuEventSystemHandler.SelectablesList;

        if (trackedButtons.Count == 0)
        {
            trackedButtons.AddRange(GetComponentsInChildren<Selectable>(true));
        }

        foreach (var btn in trackedButtons)
        {
            AddSelectionListener(btn);
        }

        targetPosition = container.anchoredPosition;
    }

    void Update()
    {
        // Smoothly move the scroll container
        container.anchoredPosition = Vector2.Lerp(
            container.anchoredPosition,
            targetPosition,
            Time.unscaledDeltaTime * moveSpeed
        );

        if (Vector2.Distance(container.anchoredPosition, targetPosition) < 0.1f)
        {
            container.anchoredPosition = targetPosition;
        }

        // Scroll wheel navigation
        float scroll = Input.mouseScrollDelta.y;
        if (scroll > 0)
        {
            Navigate(-1); // up
        }
        else if (scroll < 0)
        {
            Navigate(1); // down
        }
    }

    private void AddSelectionListener(Selectable selectable)
    {
        EventTrigger trigger = selectable.GetComponent<EventTrigger>();
        if (trigger == null) trigger = selectable.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new()
        {
            eventID = EventTriggerType.Select
        };

        entry.callback.AddListener((_) => OnButtonSelected(selectable));
        trigger.triggers.Add(entry);
    }

    private void OnButtonSelected(Selectable selectable)
    {
        if (focusTarget == null || container == null) return;

        RectTransform selectedRect = selectable.GetComponent<RectTransform>();
        if (selectedRect == null) return;

        // On first selection or reset
        if (lastSelected == null)
        {
            lastSelected = selectable;
            return;
        }

        // Measure difference between the *previous button* and *new selected one*
        RectTransform lastRect = lastSelected.GetComponent<RectTransform>();

        float deltaY = selectedRect.anchoredPosition.y - lastRect.anchoredPosition.y;

        // Apply delta to scroll container's anchored position
        targetPosition = container.anchoredPosition + new Vector2(0f, -deltaY);

        // Update lastSelected
        lastSelected = selectable;
    }

    private void Navigate(int direction)
    {
        if (trackedButtons == null || trackedButtons.Count == 0) return;

        // Get current selected button
        Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

        int index = trackedButtons.IndexOf(current);
        if (index == -1)
        {
            // If nothing is selected, default to first
            trackedButtons[0].Select();
            return;
        }

        // Move up or down
        int newIndex = Mathf.Clamp(index + direction, 0, trackedButtons.Count - 1);
        if (newIndex != index)
        {
            trackedButtons[newIndex].Select();
        }
    }
}
