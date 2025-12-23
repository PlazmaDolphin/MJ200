using UnityEngine;
using System.Linq;

public class Turret : theWall
{
    private float detectionRadius = 10f;        // radius to search for police
    private float fireCooldown = 1.5f;           // seconds between shots
    private int damagePerShot = 3;               // damage dealt per shot
    private float knockbackForce = 7f;           // knockback force applied to hit police
    [SerializeField] private SpriteRenderer gunSprite;            // sprite to rotate when firing
    
    private float lastFireTime = -1.5f;                           // time of last shot
    private PoliceLogic currentTarget;                            // currently targeted police

    void Start()
    {
        hp = 16; // Turret-specific health
    }

    void Update()
    {
        // Check if it's time to fire again
        if (Time.time - lastFireTime >= fireCooldown)
        {
            // Find all police within detection radius
            PoliceLogic[] policeInRange = FindPoliceInRange();
            
            if (policeInRange.Length > 0)
            {
                // Pick a random police from those in range
                currentTarget = policeInRange[Random.Range(0, policeInRange.Length)];
                Fire();
            }
        }
    }

    private PoliceLogic[] FindPoliceInRange()
    {
        // Get all police gameobjects and filter by distance
        return PoliceSpawner.policeList
            .Where(p => p != null && Vector2.Distance(transform.position, p.transform.position) <= detectionRadius)
            .Select(p => p.GetComponent<PoliceLogic>())
            .Where(pl => pl != null)
            .ToArray();
    }

    private void Fire()
    {
        if (currentTarget == null) return;

        // Rotate the gun sprite to point at target
        if (gunSprite != null)
        {
            Vector2 direction = (currentTarget.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            gunSprite.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Deal damage and knockback to the target
        currentTarget.damage(transform.position, knockbackForce, damagePerShot);

        lastFireTime = Time.time;
    }
}
