using System.Collections;
using UnityEngine;

public interface IHarvestable
{
    void StartHarvesting();
    void StopHarvesting();
    void CollectScrap(int amount);
}

public class ScrapPile : MonoBehaviour, IHarvestable
{
    [SerializeField]
    private float harvestTime = 3f;
    [SerializeField]
    private int currentScrapAmount = 1;

    [SerializeField]
    private Sprite smallPile, mediumPile, largePile;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private Coroutine collectCoroutine;

    [Header("Sound Effects")]
    [SerializeField] private SoundFXData collectedSound;

    // Update is called once per frame
    void Update()
    {

    }

    public void StartHarvesting()
    {
        Debug.Log("Collecting scrap...");
        if (collectCoroutine == null)
        {
            collectCoroutine = StartCoroutine(ScrapCollectingCoroutine());
        }
    }

    public void StopHarvesting()
    {
        if (collectCoroutine != null)
        {
            StopCoroutine(collectCoroutine);
            collectCoroutine = null;
            Debug.Log("Stopped collecting scrap.");
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        //if (initialScrapAmount <= 0)
        //{
        //    initialScrapAmount = Random.Range(1, 10);
        //}

        // Set sprite based on initial scrap amount, Set in Editor from 0 (empty) to 2 (full)
        UpdateSize();

        // Adjust collider size based on sprite size
        boxCollider.size = spriteRenderer.sprite.bounds.size;
    }

    public void CollectScrap(int amount = 1)
    {
        currentScrapAmount -= amount;
        InventoryManager.Instance.PickupScrap(amount);
        if (currentScrapAmount <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            UpdateSize();
        }
    }

    public void SetInitialScrap(int amount)
    {
        currentScrapAmount = amount;
        UpdateSize();
    }

    private void UpdateSize()
    {
        // Update sprite based on current scrap amount
        if (currentScrapAmount > 6)
        {
            spriteRenderer.sprite = largePile;
        }
        else if (currentScrapAmount > 3)
        {
            spriteRenderer.sprite = mediumPile;
        }
        else if (currentScrapAmount > 0)
        {
            spriteRenderer.sprite = smallPile;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.CompareTag("Player"))
        {
            //Highlight the scrap pile to indicate it can be collected from
            //(CA) Will pick this up later with a Shader change or outline effect 
        }
    }
    IEnumerator ScrapCollectingCoroutine()
    {
        yield return new WaitForSeconds(harvestTime);

        while (true)
        {
            // Collect scrap logic here
            if (collectedSound) collectedSound.Play();
            CollectScrap();
            Debug.Log("Scrap collected!");
            yield return new WaitForSeconds(harvestTime); // Collect scrap every 3 seconds
        }
    }

}
