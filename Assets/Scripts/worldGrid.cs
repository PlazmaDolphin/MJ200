using System.Collections.Generic;
using UnityEngine;

public class worldGrid : MonoBehaviour
{
    private bool gridMode = false, validPlacement = false;
    private const float placeRadius = 4f;
    private const float gridZ = 0f;
    public GameObject gridOverlay, blockGuide;
    public Transform player;
    public LayerMask gridPlaneMask;
    public GameObject wallPrefab;
    private enum Direction{Up, Left, Down, Right}
    private Direction currentDirection = Direction.Up;
    private HashSet<Vector2Int> placedBlocks = new HashSet<Vector2Int>();
    public static Vector2Int WorldToGrid(Vector3 world)
    {
        return new Vector2Int(
            Mathf.FloorToInt(world.x / GridOverlay.cellSize),
            Mathf.FloorToInt(world.y / GridOverlay.cellSize)
        );
    }

    public static Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(
            grid.x * GridOverlay.cellSize + GridOverlay.cellSize * 0.5f,
            grid.y * GridOverlay.cellSize + GridOverlay.cellSize * 0.5f,
            gridZ
        );
    }
    Vector2Int ClampGridPosToRadius(Vector2Int desiredGrid)
    {
        Vector3 desiredWorld = GridToWorld(desiredGrid);
        Vector3 playerPos = player.position;

        Vector3 offset = desiredWorld - playerPos;
        float dist = offset.magnitude;

        if (dist <= placeRadius)
            return desiredGrid;

        Vector3 clampedWorld =
            playerPos + offset.normalized * placeRadius;
        return WorldToGrid(clampedWorld);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        //toggle grid on/off with E key
        if (Input.GetKeyDown(KeyCode.E)) {
            gridOverlay.SetActive(!gridOverlay.activeSelf);
            gridMode = !gridMode;
            blockGuide.SetActive(gridMode);
        }
        // rotate blockGuide with R key
        if (gridMode && Input.GetKeyDown(KeyCode.R))
        {
            // if shift held, rotate counterclockwise
            if (Input.GetKey(KeyCode.LeftShift))
                currentDirection = (Direction)(((int)currentDirection - 1 + 4) % 4);
            else
                currentDirection = (Direction)(((int)currentDirection + 1) % 4);
        }
        Vector2Int mouseGrid = GetMouseGrid(gridZ);
        Vector2Int clampedGrid = ClampGridPosToRadius(mouseGrid);
        blockGuide.transform.position = GridToWorld(clampedGrid);
        blockGuide.transform.rotation = Quaternion.Euler(
            0f,
            0f,
            (int)currentDirection * 90f
        );
        //offset blockGuide by cellSize/2 to center it on grid cell
        //TODO: rewrite this to work with differently sized blocks
        blockGuide.transform.position = new Vector3(
            blockGuide.transform.position.x + (((int) currentDirection % 2 == 0) ? GridOverlay.cellSize * 0.5f : 0),
            blockGuide.transform.position.y + (((int) currentDirection % 2 == 1) ? GridOverlay.cellSize * 0.5f : 0),
            blockGuide.transform.position.z
        );
        validPlacement = true;
        // check if placement is valid
        validPlacement = !placedBlocks.Contains(clampedGrid) &&
            !placedBlocks.Contains((int)currentDirection % 2 == 0 ? new Vector2Int(clampedGrid.x+1, clampedGrid.y) : 
                                                                    new Vector2Int(clampedGrid.x, clampedGrid.y+1));
        // use the collider's world-space bounds to get a Vector2 center and size for Physics2D.OverlapBox
        var boxBounds = blockGuide.GetComponent<BoxCollider2D>().bounds;
        Vector2 boxCenter = new Vector2(boxBounds.center.x, boxBounds.center.y);
        Vector2 boxSize = new Vector2(boxBounds.size.x, boxBounds.size.y);
        float boxAngle = blockGuide.transform.eulerAngles.z;
        validPlacement &= Physics2D.OverlapBox(boxCenter, boxSize, boxAngle, gridPlaneMask.value) == null;
        // color blockGuide based on validity ( #B0B0FF60 for valid, #FFB0B060 for invalid)
        blockGuide.GetComponent<Renderer>().material.color = validPlacement ? new Color(0.686f, 0.686f, 1f, 0.38f) : new Color(1f, 0.686f, 0.686f, 0.38f);
        // place wall with LMB
        if (Input.GetMouseButtonDown(0) && gridMode && validPlacement)
        {
            Instantiate(wallPrefab, blockGuide.transform.position, blockGuide.transform.rotation);
            placedBlocks.Add(clampedGrid);
            //block is a 1x2 block, so add the adjacent cell to placedBlocks
            placedBlocks.Add((int)currentDirection % 2 == 0 ? new Vector2Int(clampedGrid.x+1, clampedGrid.y) : 
                                                              new Vector2Int(clampedGrid.x, clampedGrid.y+1));
        }
    }
    Vector3 GetMouseWorldPosition(float gridZ = 0f)
    {
        Vector3 mousePos = Input.mousePosition;
        Camera cam = Camera.main;

        // For orthographic camera
        if (cam.orthographic)
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            worldPos.z = gridZ; // snap to grid plane
            return worldPos;
        }

        // For perspective camera
        Ray ray = cam.ScreenPointToRay(mousePos);
        float t = (gridZ - ray.origin.z) / ray.direction.z;
        return ray.origin + ray.direction * t;
    }

    Vector2Int GetMouseGrid(float gridZ = 0f)
    {
        Vector3 worldPos = GetMouseWorldPosition(gridZ);
        return WorldToGrid(worldPos);
    }
}