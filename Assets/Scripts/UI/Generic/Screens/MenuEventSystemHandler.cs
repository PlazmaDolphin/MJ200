//using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

// This script won't be inhereted!

// Video: https://youtu.be/0EsrYNAAEEY?si=Dp-fsLru5DyHMxoR
public class MenuEventSystemHandler : MonoBehaviour
{
    public Action<Selectable> OnSelected;
    private bool hasSubscribed = false;

    [Header("References")]
    [SerializeField] private UIScreen screenController;

    [Space(20f), Header("Selectables")]
    [SerializeField] private List<Selectable> selectablesList = new();
    public List<Selectable> SelectablesList => selectablesList;
    [SerializeField] private Selectable firstSelected;
    private Selectable lastSelected;

    [Space(20f), Header("Sound FX")]
    [SerializeField] private SoundFXData selectedSFX;
    [SerializeField] private bool ignoreFirstSelectedSFX = true;
    private bool isFirstSelected;

    private void OnValidate()
    {
        if (screenController == null)
        {
            screenController = GetComponent<UIScreen>();
        }
    }

    #region INITIALIZATION
    private void OnEnable()
    {
        foreach (Selectable selectable in selectablesList)
        {
            AddSelectionListeners(selectable);
        }

        if (TryGetComponent(out screenController))
        {
            screenController.OnVisibilityChanged += OnVisibilityChanged;
            OnVisibilityChanged(screenController.IsVisible);
        }

        if (UIScreenManager.HasInstance)
        {
            UIScreenManager.Instance.OnNavigated += OnNavigate;
            UIScreenManager.Instance.OnResetNavigate += Reset_Navigation_Input;
        }

        StartCoroutine(SelectFirstElementAfterDelay(firstSelected?.gameObject)); // <<< Ensure initial selection
    }

    private void OnVisibilityChanged(bool visible)
    {
        if (visible && !hasSubscribed)
        {
            hasSubscribed = true;

            StartCoroutine(SelectFirstElementAfterDelay(firstSelected?.gameObject)); // <<< Moved here
        }
        else if (hasSubscribed)
        {
            // Equivalent to OnDisable()
            hasSubscribed = false;
        }
    }

    private void OnDisable()
    {
        if (UIScreenManager.HasInstance)
        {
            UIScreenManager.Instance.OnNavigated -= OnNavigate;
            UIScreenManager.Instance.OnResetNavigate -= Reset_Navigation_Input;
        }

        StopAllCoroutines();
        OnVisibilityChanged(false);
    }

    #region UI SCREEN MANAGER INPUT
    private void OnNavigate()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            return;
        }

        if (lastSelected == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(lastSelected.gameObject);
    }

    private void Reset_Navigation_Input()
    {
        if (firstSelected == null) return;

        // Reset into
        StartCoroutine(SelectFirstElementAfterDelay(firstSelected.gameObject));
    }
    #endregion


    private IEnumerator SelectFirstElementAfterDelay(GameObject targetSelected)
    {
        yield return null; // Let the frame finish

        // Force-clear before assigning
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(targetSelected);
        }
    }

    #endregion

    private void AddSelectionListeners(Selectable selectable)
    {
        if (!selectable.TryGetComponent(out EventTrigger eventTrigger))
        {
            eventTrigger = selectable.gameObject.AddComponent<EventTrigger>();
        }

        #region SELECT EVENT
        EventTrigger.Entry selectedEntry = new()
        {
            eventID = EventTriggerType.Select,
        };

        selectedEntry.callback.AddListener(OnSelect);
        eventTrigger.triggers.Add(selectedEntry);
        #endregion

        #region DESELECT EVENT
        EventTrigger.Entry deselectedEntry = new()
        {
            eventID = EventTriggerType.Deselect,
        };

        deselectedEntry.callback.AddListener(OnDeselect);
        eventTrigger.triggers.Add(deselectedEntry);
        #endregion

        #region POINTER ENTERS EVENT
        EventTrigger.Entry pointerEnter = new()
        {
            eventID = EventTriggerType.PointerEnter,
        };

        pointerEnter.callback.AddListener(OnButtonPointerEnter);
        eventTrigger.triggers.Add(pointerEnter);
        #endregion

        #region POINTER EXIT EVENT
        EventTrigger.Entry pointerExit = new()
        {
            eventID = EventTriggerType.PointerExit,
        };

        pointerExit.callback.AddListener(OnButtonPointerExit);
        eventTrigger.triggers.Add(pointerExit);
        #endregion
    }

    #region POINTER
    private void OnButtonPointerEnter(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;

        if (pointerEventData != null)
        {
            Selectable selectable = pointerEventData.pointerEnter.GetComponent<Selectable>();

            if (selectable == null)
            {
                selectable = pointerEventData.pointerEnter.GetComponentInChildren<Selectable>();
            }

            if (selectable == null)
            {
                selectable = pointerEventData.pointerEnter.GetComponentInParent<Selectable>();
            }

            pointerEventData.selectedObject = selectable.gameObject;
        }
    }

    private void OnButtonPointerExit(BaseEventData eventData)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;

        if (pointerEventData != null)
        {
            pointerEventData.selectedObject = null;
        }
    }
    #endregion

    #region SELECTION
    private void OnSelect(BaseEventData eventData)
    {
        if (eventData.selectedObject == null) return;

        lastSelected = eventData.selectedObject.GetComponent<Selectable>();

        if (lastSelected != null && eventData.selectedObject.TryGetComponent(out BaseButtonUI button))
        {
            button.Select();
        }

        OnSelected?.Invoke(lastSelected);

        if (selectedSFX == null)
        {
            //Debug.LogWarning($"Selected SFX is null for {gameObject.name}");
            return;
        }

        // If has sound effect, can't ignore and is the first select, don't play sound
        if (!ignoreFirstSelectedSFX && isFirstSelected)
        {
            isFirstSelected = false;
            return;
        }

        selectedSFX.Play(lastSelected.transform);
    }

    private void OnDeselect(BaseEventData eventData)
    {
        if (eventData.selectedObject == null) return;

        Selectable selectable = eventData.selectedObject.GetComponent<Selectable>();
        if (selectable != null && eventData.selectedObject.TryGetComponent(out BaseButtonUI button))
        {
            button.Deselect();
        }
    }

    #endregion

    public void SetFirstSelected(Selectable first)
    {
        StartCoroutine(SelectFirstElementAfterDelay(first.gameObject));
    }

    public void AddSelectable(Selectable newSelectable)
    {
        if (newSelectable == null)
        {
            Debug.LogWarning("Tried to add null selectable.");
            return;
        }

        if (!selectablesList.Contains(newSelectable))
        {
            selectablesList.Add(newSelectable);
            AddSelectionListeners(newSelectable);
        }
        else
        {
            Debug.LogWarning($"Selectable {newSelectable.name} is already in the list.");
        }
    }

    public void SetSelectablesList(List<Selectable> selectables)
    {
        selectablesList = selectables;
    }

    public List<Selectable> UpdateSelectablesList()
    {
        List<Selectable> selectables = GetComponentsInChildren<Selectable>().ToList();
        SetSelectablesList(selectables);
        return selectables;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MenuEventSystemHandler))]
public class MenuEventSystemHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        MenuEventSystemHandler handler = (MenuEventSystemHandler)target;

        if (GUILayout.Button("Find Buttons in Children"))
        {
            FindAndAssignButtons(handler);
        }
    }

    private void FindAndAssignButtons(MenuEventSystemHandler handler)
    {
        if (handler == null) return;

        // Find all Selectables in children (including inactive ones)
        var selectables = handler.UpdateSelectablesList();

        if (selectables.Count > 0)
        {
            Undo.RecordObject(handler, "Assign Buttons List");

            handler.SetSelectablesList(selectables);

            // Set the first selected if not already set
            handler.GetType()
                .GetField("firstSelected",
                          System.Reflection.BindingFlags.NonPublic |
                          System.Reflection.BindingFlags.Instance)
                ?.SetValue(handler, selectables[0]);

            EditorUtility.SetDirty(handler);
        }
        else
        {
            Debug.LogWarning("No Selectables found in children of " + handler.name);
        }
    }
}
#endif