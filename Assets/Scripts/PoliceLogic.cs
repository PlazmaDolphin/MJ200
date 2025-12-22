using UnityEngine;

public class PoliceLogic : MonoBehaviour
{
    public Rigidbody2D rb;
    private int health = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void damage(Vector2 hitSource, float knockbackForce, int damageAmount = 1)
    {
        Vector2 dir = ((Vector2)transform.position - hitSource).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        health -= damageAmount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
