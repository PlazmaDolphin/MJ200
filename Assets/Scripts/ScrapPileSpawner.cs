using UnityEngine;
using UnityEngine.InputSystem;

public class ScrapPileSpawner : MonoBehaviour
{

    [SerializeField] 
    private GameObject scrapPilePrefab;

    private Transform scrapPileParent;

    [SerializeField] private Transform centerTransform;

    [Header("Spawn Area settings")]
    [SerializeField] private float startRadius = 10f;
    [SerializeField] private float ringWidth = 10f;
    [SerializeField] private int ringsToSpawn = 4;
    [SerializeField] private int pilesPerRing = 3;
    [SerializeField] private int bufferBetweenRings = 2;

    [SerializeField]
    private int maxScrapPilesPerRing = 10; // Maybe adjust based on ring radius, bigger = more stacks

    [SerializeField] private int baseRingBudget = 10;
    [SerializeField] private int budgetGrowthPerRing = 8;

    [SerializeField]
    private float ringDegrees = 180f; // Degrees of the ring since we don't want a full circle
    [SerializeField]    
    private float offsetDegrees = 0f; // Rotation offset of the ring

    private float offset => offsetDegrees * Mathf.Deg2Rad;

    private int spawnedRings = 0;

    public void Awake()
    {
        scrapPileParent = new GameObject("ScrapPiles").transform;
        SpawnAllRings();
    }
    public void Update()
    {
        if (Keyboard.current.oKey.wasPressedThisFrame)
        {
            Destroy(scrapPileParent.gameObject);
            scrapPileParent = new GameObject("ScrapPiles").transform;
            SpawnAllRings();
        }
    }


    public void SpawnAllRings()
    {
        Vector3 center = centerTransform ? centerTransform.position : Vector3.zero;

        for (int ringIndex = 0; ringIndex < ringsToSpawn; ringIndex++)
        {
            SpawnRing(center, ringIndex);
        }
    }

    private void SpawnRing(Vector3 center, int ringIndex)
    {
        float inner = ringIndex * ringWidth + startRadius + bufferBetweenRings*ringIndex;
        float outer = inner + ringWidth;

        int ringBudget = baseRingBudget + ringIndex * budgetGrowthPerRing;
        int remainingBudget = ringBudget - (pilesPerRing+ringIndex) * 2;

        for (int i = 0; i < pilesPerRing+ringIndex; i++)
        {
            Vector3 candidate = center + RandomPointInAnnulus(inner, outer);

            GameObject prefab = Instantiate(scrapPilePrefab, candidate, Quaternion.identity,scrapPileParent);
            

            int value = PickStackValue(remainingBudget, ringIndex, i == pilesPerRing+ringIndex-1);

            prefab.GetComponent<ScrapPile>().SetInitialScrap(value + 2);

            remainingBudget -= value;
            if (remainingBudget <= 0)
                break;
        }

        InventoryManager.Instance.AddWorldScrap(ringBudget - remainingBudget);

        Debug.Log($"remaining {remainingBudget}");
    }

    private Vector3 RandomPointInAnnulus(float inner, float outer)
    {
        // uniform over oppervlakte:
        float t = Random.value;
        float r = Mathf.Sqrt(Mathf.Lerp(inner * inner, outer * outer, t));

        float sectorSizeRad = ringDegrees * Mathf.Deg2Rad;
        float startAngle = offset - sectorSizeRad * 0.5f;
        float angle = startAngle + Random.value * sectorSizeRad;

        return new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
    }

    private int PickStackValue(int remainingBudget, int ringIndex, bool dumpRemaining = false)
    {
        if (remainingBudget <= 0) return 0;

        // Max stack size increases with ring index, but is capped by remaining budget
        int max = Mathf.Max(1, Mathf.Min(remainingBudget, 8 + ringIndex));
        if (dumpRemaining)
        {
            return remainingBudget;
        }
        return Random.Range(1, max + 1);
    }
}
