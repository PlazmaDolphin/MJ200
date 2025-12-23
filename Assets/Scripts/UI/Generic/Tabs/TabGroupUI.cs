using System;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupUI : MonoBehaviour
{
    public Action<int> OnChangedSelectedTab;

    [Header("Tabs")]
    [SerializeField] private bool enableLooping = false;
    [SerializeField] private int initialScreen = 0;
    [SerializeField] private List<UIScreen> screensList = new();
    [ReadOnly, SerializeField] private int currentIndex = -1;
    public List<UIScreen> ScreensList => screensList;
    public int CurrentIndex => currentIndex;
    public UIScreen PreviousScreen { get; private set; }

    private void OnValidate()
    {
        screensList.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (!child.TryGetComponent(out UIScreen uiScreen))
            {
                continue;
            }

            screensList.Add(uiScreen);
        }
    }

    private void Awake()
    {
        foreach (var screen in screensList)
        {
            screen.SetImmediately(screen == screensList[initialScreen]);
        }

        currentIndex = initialScreen;
    }

    public void Next() => MoveTab(1);
    public void Previous() => MoveTab(-1);

    public void MoveTab(int amountToMove)
    {
        int targetIndex = currentIndex + amountToMove;
        int max = screensList.Count - 1;
        int min = 0;

        if (targetIndex < min)
        {
            if (enableLooping)
            {
                targetIndex = max;
            }
            else
            {
                return;
            }
        }
        else if (targetIndex > max)
        {
            if (enableLooping)
            {
                targetIndex = min;
            }
            else
            {
                return;
            }
        }

        SelectScreen(targetIndex);
    }

    public void SelectScreen(int targetIndex)
    {
        if (currentIndex == targetIndex)
        {
            Debug.Log($"Cant set to same index! current: {currentIndex}; target {targetIndex}");
            return;
        }

        // Store the old screen before changing index
        PreviousScreen = screensList[currentIndex];

        currentIndex = targetIndex;
        OnChangedSelectedTab?.Invoke(currentIndex);
        Debug.Log($"Selected {currentIndex}");

        if (PreviousScreen != null)
        {
            PreviousScreen.SetWithTransition(false);
        }

        screensList[currentIndex].SetWithTransition(true);
    }
}