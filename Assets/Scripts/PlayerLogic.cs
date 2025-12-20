using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera cam;
    public Collider2D playerCollider;
    public TextMeshProUGUI scrapText; // TODO: Replace with proper UI element
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
        input.Normalize();
        // Move the player based on input
        transform.Translate(input * speed * Time.deltaTime, Space.World);
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
