using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Camera cam;
    public Collider2D playerCollider;
    //public TextMeshProUGUI scrapText; // TODO: Replace with proper UI element
    public SpriteRenderer playerSprite;
    public WeaponLogic weapon;
    private float speed = 5f;
    private float initXScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initXScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // flip player based on mouse position
        transform.localScale = new Vector3(initXScale * (Input.mousePosition.x < Screen.width / 2 ? -1 : 1), transform.localScale.y, transform.localScale.z);
        // handle weapon usage
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

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("scrap"))
        {
            other.gameObject.GetComponent<ScrapLogic>().Die();
        }
    }
}
