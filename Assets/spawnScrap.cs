using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class spawnScrap : MonoBehaviour
{
    public static List<GameObject> scrapList = new List<GameObject>();
    //each scrap has a 2D collider
    private const int GLOBAL_SCRAP_LIMIT = 50; //scraps the game attempts to spawn
    private int scrapsSpawned = 0; //scraps actually spawned
    private Vector2 spawnBounds = new Vector2(30f, 30f);
    private float minDistanceBetweenScrap = 1f;
    private const int MAX_SPAWN_ATTEMPTS = 10;
    public GameObject scrapPrefab;
    public AnimationCurve spawnBiasX, spawnBiasY;
    public TextMeshProUGUI globalScrapCountText;
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
            if (tooClose){
                Debug.Log("Spawn failed, position: " + newPos);   
                continue;
            }
            GameObject newScrap = Instantiate(scrapPrefab, newPos, Quaternion.identity);
            scrapList.Add(newScrap);
        }
        scrapsSpawned = scrapList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        globalScrapCountText.text = "World Scrap Left:\n" + scrapList.Count + "/" + scrapsSpawned;
    }
    public static void RemoveScrap(GameObject scrap)
    {
        scrapList.Remove(scrap);
    }
}
