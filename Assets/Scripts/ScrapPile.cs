using UnityEngine;

public class ScrapPile : MonoBehaviour
{
    [SerializeField]
    private int currentScrapAmount = 1;

    public int initialScrapAmount = 0;

    [SerializeField]
    private Sprite smallPile, mediumPile, largePile;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (initialScrapAmount <= 0)
        {
            initialScrapAmount = Random.Range(1, 10);
        }
        currentScrapAmount = initialScrapAmount;
        
        // Set sprite based on initial scrap amount, Set in Editor from 0 (empty) to 2 (full)
        UpdateSize();

        // Adjust collider size based on sprite size
        boxCollider.size = spriteRenderer.sprite.bounds.size;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void CollectScrap(int amount)
    {
        currentScrapAmount -= amount;
        if (currentScrapAmount <= 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            UpdateSize();
        }
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
}
