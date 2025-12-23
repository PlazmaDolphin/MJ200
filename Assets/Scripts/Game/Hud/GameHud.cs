using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    PlayerController player;
    PoliceSpawner policeSpawner;

    [SerializeField] private TextMeshProUGUI playerHealthText;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private string dayFormat = "Day: {0}";
    [SerializeField] private Image timeFillImage;

    [SerializeField] private Color normalColor = Color.blue;
    [SerializeField] private Color raidColor = Color.red;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        policeSpawner = PoliceSpawner.Instance;

        player.OnHealthChanged += UpdateHealthUI;

        policeSpawner.OnDayChanged += UpdateDayUI;
        policeSpawner.OnRaidingStateChanged += UpdateRaidStateUI;

        UpdateHealthUI(player.currentHealth);
        UpdateDayUI(policeSpawner.CurrentDay);
        UpdateRaidStateUI(policeSpawner.IsRaiding);
    }

    private void Update()
    {
        UpdateTimeFill();
    }

    private void UpdateHealthUI(int health)
    {
        playerHealthText.text = health.ToString();
    }

    private void UpdateDayUI(int day)
    {
        dayText.text = string.Format(dayFormat, day);
    }

    private void UpdateRaidStateUI(bool raiding)
    {
        timeFillImage.color = raiding ? raidColor : normalColor;
    }

    private void UpdateTimeFill()
    {
        timeFillImage.fillAmount = policeSpawner.GetDayProgress01();
    }
}
