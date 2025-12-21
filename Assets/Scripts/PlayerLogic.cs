using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera cam;
    public Collider2D playerCollider;
    public TextMeshProUGUI scrapText; // TODO: Replace with proper UI element
    public SpriteRenderer playerSprite;
    private float speed = 5f;
    private int scrapCollected = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Get WASD input and normalize
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // flip sprite based on input
        if (input.x < 0)
            playerSprite.flipX = true;
        else if (input.x > 0)
            playerSprite.flipX = false;
        input.Normalize();
        // Move the player based on input
        transform.Translate(input * speed * Time.deltaTime, Space.World);
        // force player rotation to be 0
        transform.rotation = Quaternion.Euler(0, 0, 0);
        // center camera to player position
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);
        scrapText.text = "Scrap: " + scrapCollected;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        if (other.CompareTag("scrap"))
        {
            scrapCollected++;
            other.gameObject.GetComponent<ScrapLogic>().Die();
        }
    }
}
