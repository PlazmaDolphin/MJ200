using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    public Action<bool> OnBuildingStateChanged;

    public PlayerMovement Movement { get; private set; }

    [Header("Scrap")]
    //public TextMeshProUGUI scrapText; // TODO: Replace with proper UI element
    public int scrapCollected = 0;
    public bool isBuilding;

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Replace this with the actual key we want for building
        if (Input.GetKey(KeyCode.Space))
        {
            SetIsBuilding(true);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            SetIsBuilding(false);
        }
    }

    private void SetIsBuilding(bool isBuilding)
    {
        // Prevent from calling multiple times
        if (this.isBuilding == isBuilding) return;

        // Can only move if not building
        Movement.SetCanMove(!isBuilding);
        OnBuildingStateChanged?.Invoke(isBuilding);
        this.isBuilding = isBuilding;
    }
}