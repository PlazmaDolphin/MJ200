using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scrapText;
    [SerializeField]
    private TextMeshProUGUI totalScrapText;
    // Simple singleton pattern to ensure only one instance of InventoryManager exists and we get access to it globally
    private static InventoryManager _instance;

    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindFirstObjectByType<InventoryManager>();
            }

            return _instance;
        }
    }

    private int totalWorldScrap = 0;

    private int scrapCount = 0;
    //Using property to access scrapCount to keep it save
    public int ScrapCount
    {
        get { return scrapCount; }
    }

    //Possible future expansion to add second currency

    private void Start()
    {
        scrapText.text = "Scrap: " + scrapCount.ToString();
        totalScrapText.text = "Total World Scrap: " + totalWorldScrap.ToString();
    }

    /// <summary>
    /// Simple function to add scrap to the inventory from the world.
    /// </summary>
    /// <param name="amount"></param>
    public void PickupScrap(int amount = 1)
    {
        scrapCount += amount;
        scrapText.text = "Scrap: " + scrapCount.ToString();
        AddWorldScrap(-amount);
    }

    /// <summary>
    /// Situations were we don't want to update total world scrap, just remove from player inventory.
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveScrap(int amount = 1)
    {
        scrapCount -= amount;
        if (scrapCount < 0) scrapCount = 0;
        scrapText.text = "Scrap: " + scrapCount.ToString();
    }

    public void AddWorldScrap(int amount = 1)
    {
        totalWorldScrap += amount;
        totalScrapText.text = "Total World Scrap: " + totalWorldScrap.ToString();
    }
}
