using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    public GameObject knife;
    private const float KNIFE_DURATION = 0.25f;
    private const float KNIFE_COOLDOWN = 0.4f;
    private float lastStab;
    // TODO: Add other weapons than knife
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastStab = Time.time - KNIFE_COOLDOWN;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UseWeapon()
    {
        // if cooldown is active, do nothing
        if (Time.time - lastStab < KNIFE_COOLDOWN)
            return;
        // Get mouse angle relative to center of screen
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float angle = Mathf.Atan2(mousePos.y - screenCenter.y, mousePos.x - screenCenter.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        knife.SetActive(true);
        lastStab = Time.time;
        Invoke(nameof(EndAttack), KNIFE_DURATION);
    }
    private void EndAttack()
    {
        knife.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("police"))
        {
            collision.gameObject.GetComponent<PoliceLogic>().damage(transform.position, 20f, 1);
        }
    }
}
