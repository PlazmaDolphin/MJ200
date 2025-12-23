using System.Collections.Generic;
using UnityEngine;

public class SpawnScrap : MonoBehaviour
{
    private List<GameObject> scrapList = new List<GameObject>();
    //each scrap has a 2D collider
    private const int GLOBAL_SCRAP_LIMIT = 20; //scraps the game attempts to spawn
    private int scrapsSpawned = 0; //scraps actually spawned
    private Vector2 spawnBounds = new Vector2(30f, 30f);
    private float minDistanceBetweenScrap = 1f;
    private const int MAX_SPAWN_ATTEMPTS = 10;
    public GameObject scrapPrefab;
    public AnimationCurve spawnBiasX, spawnBiasY;

    //(CA) Note: The spawning is really unoptimized. I can improve it later if you want. The brute force spawning and checking all other objects for distances scales really poorly.
    //Fine for now since the scrap limit is low.
    public GameObject scrapRoot;

    private void Awake()
    {
        //Put it in a parent so it's less messy in the hierarchy
        scrapRoot = new GameObject("ScrapRoot");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //spawn scrap at random positions within the bounds
        for (int i = 0; i < GLOBAL_SCRAP_LIMIT; i++)
        {
            Vector3 newPos = Vector3.zero;
            bool tooClose = true;
            //check that new scrap is not too close to existing scrap, using colliders
            for (int j = 0; j < MAX_SPAWN_ATTEMPTS; j++)
            {
                tooClose = false;
                //spawn scrap at random position, baised towards the center of the bounds
                //newPos = new Vector3(Random.Range(0, 1), Random.Range(0, 1), 0);
                //newPos.x = spawnBounds.x * spawnBiasX.Evaluate(newPos.x);
                //newPos.y = spawnBounds.y * spawnBiasY.Evaluate(newPos.y);
                newPos = new Vector3(spawnBounds.x * spawnBiasX.Evaluate(Random.value), spawnBounds.y * spawnBiasY.Evaluate(Random.value), 0);
                foreach (GameObject existingScrap in scrapList)
                {
                    if (Vector2.Distance(newPos, existingScrap.transform.position) < minDistanceBetweenScrap)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (!tooClose) break;
            }
            if (tooClose)
            {
                Debug.Log("Spawn failed, position: " + newPos);
                continue;
            }
            GameObject newScrap = Instantiate(scrapPrefab, newPos, Quaternion.identity);
            newScrap.transform.parent = scrapRoot.transform;
            scrapList.Add(newScrap);
        }
        scrapsSpawned = scrapList.Count;
        InventoryManager.Instance.AddWorldScrap(scrapsSpawned);
    }
}
