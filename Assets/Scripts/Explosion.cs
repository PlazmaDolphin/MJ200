using UnityEngine;

public class Explosion : MonoBehaviour
{
    private const float KNOCKBACK = 50f;
    private const int DAMAGE = 6;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(die), 0.6f);
    }
    void die(){ Destroy(gameObject); }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("police"))
        {
            other.gameObject.GetComponent<PoliceLogic>().damage(transform.position, KNOCKBACK, DAMAGE);
        }
    }
}
