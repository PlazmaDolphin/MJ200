using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody2D rb;
    private float knockback;
    private int damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Sound Effects")]
    [SerializeField] private SoundFXData hitSound;

    void Start()
    {
        Invoke(nameof(die), 2); // destroy bullet after 2 seconds
    }
    private void die() { Destroy(gameObject); }
    // Update is called once per frame
    void Update()
    {

    }
    public void initBullet(Vector2 velocity, int damage, float knockback)
    {
        rb.linearVelocity = velocity;
        this.damage = damage;
        this.knockback = knockback;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("police"))
        {
            collision.gameObject.GetComponent<PoliceLogic>().damage(transform.position, knockback, damage);

            if (hitSound) hitSound.Play();

            Destroy(gameObject);
        }
    }
}
