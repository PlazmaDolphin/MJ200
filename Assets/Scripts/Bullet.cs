using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    private float knockback;
    private int damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(die), 2); // destroy bullet after 2 seconds
    }
    private void die(){ Destroy(gameObject); }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void initBullet(Vector2 velocity, int damage, float knockback)
    {
        rb.linearVelocity = velocity;
        this.damage = damage;
        this.knockback = knockback;

        if (rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("police"))
        {
            collision.gameObject.GetComponent<PoliceLogic>().damage(transform.position, knockback, damage);
            Destroy(gameObject);
        }
    }
}
