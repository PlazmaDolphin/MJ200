using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class PoliceLogic : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public NavMeshAgent agent;
    private theWall targetWall;
    private int health = 10;
    private bool pursuingWall = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Sound Effects")]
    [SerializeField] private SoundFXData hurtSound;
    [SerializeField] private SoundFXData deathSound;

    void Start()
    {
        // force z-position to zero

        testPath = new NavMeshPath();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Update is called once per frame
    private NavMeshPath testPath;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        if (pursuingWall)
        {
            // If the wall reference was destroyed, stop pursuing it.
            if (targetWall == null)
            {
                pursuingWall = false;
            }

            // If we're in range of the wall, attempt to hit it.
            if (pursuingWall && agent.remainingDistance < 0.5f)
            {
                if (targetWall.AttemptHit())
                {
                    Debug.Log("Wall destroyed");
                    // Wall destroyed — stop pursuing and let next loop handle player
                    pursuingWall = false;
                    targetWall = null;
                }
                else
                {
                    // Wall still exists; remain committed
                    return;
                }
            }

            // If still pursuing, check whether the wall is reachable via NavMesh.
            if (pursuingWall)
            {
                var wallPos = targetWall != null ? targetWall.transform.position : (Vector3?)null;
                if (wallPos.HasValue)
                {
                    var wallPath = new NavMeshPath();
                    NavMesh.CalculatePath(agent.transform.position, wallPos.Value, NavMesh.AllAreas, wallPath);
                    if (wallPath.status != NavMeshPathStatus.PathComplete)
                    {
                        // Wall became unreachable; abandon it
                        pursuingWall = false;
                        targetWall = null;
                    }
                    else
                    {
                        // Still reachable and not in range yet — continue toward it
                        return;
                    }
                }
                else
                {
                    pursuingWall = false;
                }
            }
        }
        else ReEvaluate();
    }
    void ReEvaluate()
    {
        NavMeshHit agentHit;
        NavMeshHit playerHit;

        bool agentOnMesh = NavMesh.SamplePosition(
            agent.transform.position,
            out agentHit,
            1.0f,
            NavMesh.AllAreas
        );

        bool playerOnMesh = NavMesh.SamplePosition(
            PlayerMovement.playerPos.position,
            out playerHit,
            1.0f,
            NavMesh.AllAreas
        );

        testPath.ClearCorners();

        bool success = NavMesh.CalculatePath(
            agentHit.position,
            playerHit.position,
            NavMesh.AllAreas,
            testPath
        );

        Debug.Log($"Calc={success}, Status={testPath.status}, Corners={testPath.corners.Length}");
        if (testPath.status == NavMeshPathStatus.PathComplete)
        {
            // NEVER BECOMES TRUE, EVALUATE
            Debug.Log("Player reachable, pursuing");
            pursuingWall = false;
            targetWall = null;
            agent.SetDestination(PlayerMovement.playerPos.position);
        }
        else
        {
            if (!pursuingWall) pursueWall();
        }
    }

    // Select a wall to attack. Prefer the wall directly between AI and player
    // (raycast); otherwise choose the nearest wall by distance.
    private void pursueWall()
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, (PlayerMovement.playerPos.position - transform.position).normalized, 100);

        // check if the hit is a wall
        if (hit.collider != null && hit.collider.CompareTag("wall"))
        {
            agent.SetDestination(hit.point);
            targetWall = hit.collider.gameObject.GetComponent<theWall>();
            pursuingWall = true;
            return;
        }

        GameObject[] walls = GameObject.FindGameObjectsWithTag("wall");
        if (walls.Length == 0) return;
        // pick nearest wall
        var nearest = walls.OrderBy(w => Vector2.Distance(transform.position, w.transform.position)).First();
        agent.SetDestination(nearest.transform.position);
        sprite.flipX = nearest.transform.position.x < 0;
        targetWall = nearest.GetComponent<theWall>();
        pursuingWall = true;
    }
    public void damage(Vector2 hitSource, float knockbackForce, int damageAmount = 1)
    {
        if (hurtSound) hurtSound.Play();

        Vector2 dir = ((Vector2)transform.position - hitSource).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        health -= damageAmount;
        if (health <= 0)
        {
            if (deathSound) deathSound.Play();
            PoliceSpawner.policeList.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
