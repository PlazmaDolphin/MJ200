using UnityEngine;

public class Grenade : MonoBehaviour
{
    private bool active, grounded;
    private const int DIRECT_HIT_DAMAGE = 2;
    private const float EXPLODE_TIME = 0.5f;
    public const float ACTIVATION_PROGRESS = 0.7f;
    public GameObject explosion;

    [Header("Sound Effects")]
    [SerializeField] private SoundFXData explodeSound;

    public void activate()
    {
        active = true;
        //turn on hitbox
        GetComponent<Collider2D>().enabled = true;
    }
    public void ground()
    {
        grounded = true;
        Invoke(nameof(explode), EXPLODE_TIME);
    }
    private void explode()
    {
        if (explodeSound) explodeSound.Play();

        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!active || grounded) return;
        if (other.gameObject.CompareTag("police"))
        {
            //deal extra damage for direct hit
            other.gameObject.GetComponent<PoliceLogic>().damage(Vector2.zero, 0f, DIRECT_HIT_DAMAGE);
            explode();
        }
    }
}
