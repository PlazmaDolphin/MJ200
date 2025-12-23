using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    public Action<int> OnHealthChanged;
    public Action<bool> OnBuildingStateChanged;
    public Action<bool> OnSalvagingStateChanged;

    public Rigidbody2D Rb { get; private set; }
    public PlayerMovement Movement { get; private set; }

    [Header("Weapons")]
    private IHarvestable currentHarvestable;
    public WeaponLogic weapon;
    public bool isBuilding, isHarvesting;

    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;
    [SerializeField] private UnityEvent onLose;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource buildingAudio;
    [SerializeField] private SoundFXData collectScrapSound;

    [Space(10f), SerializeField] private SoundFXData hurtSound;
    [SerializeField] private SoundFXData healSound;
    [SerializeField] private SoundFXData deathSound;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Movement = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        // Replace this with the actual key we want for building
        if (Keyboard.current.eKey.wasPressedThisFrame)
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

        if (Keyboard.current.fKey.isPressed)
            SetIsHarvesting(true);
        else if (Keyboard.current.fKey.wasReleasedThisFrame)
            SetIsHarvesting(false);

    }

    private void SetIsHarvesting(bool isHarvesting)
    {
        if (isBuilding) return; // Can't harvest while building
        // If there's no harvestable object anymore, do nothing
        if (currentHarvestable == null && isHarvesting)
        {
            //Stop harvesting if we were harvesting
            SetIsHarvesting(false);
            return;
        }
        // Can only move if not harvesting
        Movement.SetCanMove(!isHarvesting);
        if (isHarvesting)
            currentHarvestable?.StartHarvesting();
        else
            currentHarvestable?.StopHarvesting();
        OnSalvagingStateChanged?.Invoke(isHarvesting);
        this.isHarvesting = isHarvesting;
    }

    private void SetIsBuilding(bool isBuilding)
    {
        // Prevent from calling multiple times
        if (this.isBuilding == isBuilding) return;
        if (isHarvesting) return;

        buildingAudio.gameObject.SetActive(isBuilding);

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
            if (collectScrapSound) collectScrapSound.Play();

            collision.gameObject.GetComponent<ScrapLogic>().Die();
        }
        if (collision.CompareTag("police"))
        {
            if (hurtSound) hurtSound.Play();

            // Take Damage
            SetCurrentHealth(currentHealth - 1);
            OnHealthChanged?.Invoke(currentHealth);

            if (currentHealth <= 0)
            {
                if (deathSound) deathSound.Play();

                Debug.Log("Game Over");
                // Implement game over logic here
                onLose.Invoke();
            }
            //apply knockback
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            Rb.AddForce(knockbackDirection * 20f, ForceMode2D.Impulse);
        }
    }

    public void Heal()
    {
        if (healSound) healSound.Play();

        SetCurrentHealth(maxHealth);
    }

    public void SetCurrentHealth(int amount)
    {
        currentHealth = amount;
        OnHealthChanged?.Invoke(amount);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IHarvestable harvestable) && harvestable == currentHarvestable)
            currentHarvestable = null;
    }
}