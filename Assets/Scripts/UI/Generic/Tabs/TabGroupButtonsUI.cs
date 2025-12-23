using System;
using System.Collections.Generic;
using UnityEngine;

public class TabGroupButtonsUI : MonoBehaviour
{
    //InputManager instance;

    [Header("Buttons")]
    [SerializeField] private TabGroupUI tabGroupUI;
    [SerializeField] private TabButtonUI[] tabButtons;
    [SerializeField] private BaseButtonUI leftTabButton;
    [SerializeField] private BaseButtonUI rightTabButton;
    private TabButtonUI currentTab;

    private readonly Dictionary<TabButtonUI, Action> buttonHandlers = new();

    private void OnValidate()
    {
        // Get childs
        List<TabButtonUI> tabButtons = new();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (!child.TryGetComponent(out TabButtonUI baseButton))
            {
                continue;
            }

            if (baseButton == leftTabButton) continue;
            if (baseButton == rightTabButton) continue;

            tabButtons.Add(baseButton);
        }

        this.tabButtons = tabButtons.ToArray();
    }

    private void Start()
    {
        tabGroupUI.OnChangedSelectedTab += OnChangedSelectedTab;

        //leftTabButton.OnEndedAnimation += tabGroupUI.Previous;
        //rightTabButton.OnEndedAnimation += tabGroupUI.Next;

        //instance = InputManager.Instance;

        //if (instance != null)
        //{
        //    instance.leftTabInput.OnPerformed += tabGroupUI.Previous;
        //    instance.rightTabInput.OnPerformed += tabGroupUI.Next;
        //}

        for (int i = 0; i < tabButtons.Length; i++)
        {
            var tabButton = tabButtons[i];
            int index = i;

            // Create and store the handler so it can be unsubscribed later
            Action handler = () => OnTabButtonClicked(index);
            buttonHandlers[tabButton] = handler;

            tabButton.Button.OnEndedAnimation += handler;
            //tabButton.Background.color = i == tabGroupUI.CurrentIndex ? selectedColor : normalColor;
        }

        currentTab = tabButtons[tabGroupUI.CurrentIndex];
    }

    private void OnDestroy()
    {
        //if (instance != null)
        //{
        //    instance.leftTabInput.OnPerformed -= tabGroupUI.Previous;
        //    instance.rightTabInput.OnPerformed -= tabGroupUI.Next;
        //}

        // Unsubscribe all stored handlers
        foreach (var pair in buttonHandlers)
        {
            pair.Key.Button.OnEndedAnimation -= pair.Value;
        }

        buttonHandlers.Clear();
    }

    private void OnTabButtonClicked(int index)
    {
        Debug.Log($"Button {index} pressed");
        tabGroupUI.SelectScreen(index);
    }

    private void OnChangedSelectedTab(int index)
    {
        if (currentTab != null)
        {
            currentTab.SetState(false);
        }

        currentTab = tabButtons[index];
        currentTab.SetState(true);
    }
}
