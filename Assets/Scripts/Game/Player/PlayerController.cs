using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    private IHarvestable currentHarvestable;

    public Action<bool> OnBuildingStateChanged;
    public Action<bool> OnSalvagingStateChanged;

    public WeaponLogic weapon;

    public PlayerMovement Movement { get; private set; }

    public bool isBuilding, isHarvesting;

    private void Awake()
    {
        Movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Replace this with the actual key we want for building
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SetIsBuilding(!isBuilding);
        }

        if (Input.GetMouseButtonDown(0) && !worldGrid.gridMode)
        {
            weapon.UseWeapon();
        }
        //scrapText.text = "Scrap: " + InventoryManager.Instance.ScrapCount;
        // scroll wheel to change weapon if not in grid mode
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !worldGrid.gridMode)
        {
            weapon.SwitchWeapon(Input.GetAxis("Mouse ScrollWheel") > 0 ? 1 : -1);
        }

        SetIsHarvesting(Keyboard.current.fKey.isPressed);

    }

    private void SetIsHarvesting(bool isHarvesting)
    {
        // Prevent from calling multiple times
        if (this.isHarvesting == isHarvesting) return;
        // Can only move if not harvesting
        Movement.SetCanMove(!isHarvesting);
        if (isHarvesting)
            currentHarvestable?.StartHarvesting();
        else
            currentHarvestable?.StopHarvesting();
        OnSalvagingStateChanged?.Invoke(isBuilding);
        this.isHarvesting = isHarvesting;
    }

    private void SetIsBuilding(bool isBuilding)
    {
        // Prevent from calling multiple times
        if (this.isBuilding == isBuilding) return;

        // Can only move if not building (we can ofcourse change this eaasily)
        Movement.SetCanMove(!isBuilding);
        OnBuildingStateChanged?.Invoke(isBuilding);
        this.isBuilding = isBuilding;
    }


    //Whenever we enter a harvestable object's collider, we set it as the current harvestable
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHarvestable harvestable))
            currentHarvestable = harvestable;

        if (collision.CompareTag("scrap"))
        {
            collision.gameObject.GetComponent<ScrapLogic>().Die();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHarvestable harvestable) && harvestable == currentHarvestable)
            currentHarvestable = null;
    }


}