using System.Collections.Generic;
using UnityEngine;

public class theWall : MonoBehaviour
{
    private int hp = 8;
    private float lastHitTime;
    private const float TIME_BETWEEN_HITS = 0.8f;
    public List<Vector2Int> gridPositions = new List<Vector2Int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateGrid(true);
        lastHitTime = Time.time - 100f;
    }
    public bool AttemptHit()
    {
        if (Time.time - lastHitTime < TIME_BETWEEN_HITS)
            return false;
        lastHitTime = Time.time;
        hp--;
        if (hp <= 0)
        {
            updateGrid(false);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
    private void updateGrid(bool created)
    {
        if (created)
        {
            // update logic to show face sprites
        }
        else
        {
            foreach (Vector2Int pos in gridPositions)
            {
                worldGrid.placedBlocks.Remove(pos);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
