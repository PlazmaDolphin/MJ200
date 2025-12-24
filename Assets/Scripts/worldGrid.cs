using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class worldGrid : MonoBehaviourSingleton<worldGrid>
{
    public bool gridMode = false;
    private bool validPlacement = false;
    [SerializeField] private float placeRadius = 4f;
    [SerializeField] private float gridZ = 0f;
    public GameObject gridOverlay, blockGuide;
    public Transform player;
    public LayerMask gridPlaneMask;
    public GameObject wallPrefab, turretPrefab;
    public Sprite[] blockSprites; // Array of sprites for different block sizes

    private GameObject wallBlockHolder;

    private enum Direction { Up, Left, Down, Right }
    private Direction currentDirection = Direction.Up;
    private int currentBlockSize = 0;

    // Define block dimensions for each size: (width, height)
    private Vector2Int[] blockDimensions = new Vector2Int[]
    {
        new Vector2Int(1, 1), // Size1x1
        new Vector2Int(2, 1), // Size1x2
        new Vector2Int(2, 2), // Size2x2
        new Vector2Int(3, 2)  // Size2x3
    };

    private void Start()
    {
        gridMode = false;
        wallBlockHolder = new GameObject("WallBlocks");
        placedBlocks = new HashSet<Vector2Int>();
    }

    public static HashSet<Vector2Int> placedBlocks = new HashSet<Vector2Int>();
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
            Instance.gridZ
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

    // Update is called once per frame
    void Update()
    {
        if (!GameStateManager.CanPlay()) return;

        //// rotate blockGuide with R key
        //if (gridMode && Input.GetKeyDown(KeyCode.R))
        //{
        //    // if shift held, rotate counterclockwise
        //    if (Input.GetKey(KeyCode.LeftShift))
        //        currentDirection = (Direction)(((int)currentDirection - 1 + 4) % 4);
        //    else
        //        currentDirection = (Direction)(((int)currentDirection + 1) % 4);
        //}

        if (gridMode)
        {
            // change block size with scroll wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (gridMode && scroll != 0)
            {
                currentBlockSize += scroll > 0 ? 1 : -1;
                currentBlockSize %= blockDimensions.Length;
                if (currentBlockSize == -1) currentBlockSize = blockDimensions.Length - 1;
                blockGuide.GetComponent<SpriteRenderer>().sprite = blockSprites[currentBlockSize];
            }
            Vector2Int mouseGrid = GetMouseGrid(gridZ);
            Vector2Int clampedGrid = ClampGridPosToRadius(mouseGrid);
            blockGuide.transform.position = GridToWorld(clampedGrid);
            blockGuide.transform.rotation = Quaternion.Euler(
                0f,
                0f,
                (int)currentDirection * 90f
            );
            // offset blockGuide based on block size and direction
            Vector2Int blockDims = GetRotatedBlockDimensions();
            blockGuide.transform.position = new Vector3(
                blockGuide.transform.position.x + (blockDims.x - 1) * GridOverlay.cellSize * 0.5f,
                blockGuide.transform.position.y + (blockDims.y - 1) * GridOverlay.cellSize * 0.5f,
                blockGuide.transform.position.z
            );

            validPlacement = true;

            // check if placement is valid (no overlapping with placed blocks)
            validPlacement = !DoesBlockOverlap(clampedGrid);
            validPlacement = CanPayForBlock();

            // use the collider's world-space bounds to get a Vector2 center and size for Physics2D.OverlapBox
            var boxBounds = blockGuide.GetComponent<BoxCollider2D>().bounds;
            Vector2 boxCenter = new Vector2(boxBounds.center.x, boxBounds.center.y);
            Vector2 boxSize = new Vector2(boxBounds.size.x, boxBounds.size.y);
            float boxAngle = blockGuide.transform.eulerAngles.z;
            validPlacement &= Physics2D.OverlapBox(boxCenter, boxSize, boxAngle, gridPlaneMask.value) == null;

            Collider2D[] hit = Physics2D.OverlapBoxAll(boxCenter, boxSize, boxAngle);
            foreach (var item in hit)
            {
                Debug.Log(item.name);
            }

            // color blockGuide based on validity ( #B0B0FF60 for valid, #FFB0B060 for invalid)
            blockGuide.GetComponent<Renderer>().material.color = validPlacement ? new Color(0.686f, 0.686f, 1f, 0.5f) : new Color(1f, 0.686f, 0.686f, 0.5f);

            // place wall with LMB
            if (Input.GetMouseButtonDown(0) && gridMode && validPlacement)
            {
                var buildingCost = 0;

                GameObject newWall = Instantiate(currentBlockSize == 2 ? turretPrefab : wallPrefab, blockGuide.transform.position, blockGuide.transform.rotation, wallBlockHolder.transform);
                newWall.GetComponent<SpriteRenderer>().sprite = blockSprites[currentBlockSize];
                // Update collider to match block size
                BoxCollider2D collider = newWall.GetComponent<BoxCollider2D>();
                if (collider != null)
                {
                }

                // If a NavMeshObstacle exists on the prefab, resize it to match the placed block
                NavMeshObstacle navObs = newWall.GetComponent<NavMeshObstacle>();
                if (navObs != null)
                {
                    Vector2Int dims = blockDimensions[currentBlockSize];
                    collider.size = new Vector2(dims.x, dims.y);
                    Vector2Int rotatedDims = GetRotatedBlockDimensions();
                    // keep existing z size (thickness) if any
                    Vector3 obsSize = navObs.size;
                    // Map grid width -> X, grid height -> Z (NavMesh uses XZ plane)
                    obsSize.x = dims.x;
                    obsSize.y = dims.y;
                    navObs.size = obsSize;
                    // keep vertical center as-is; zero X/Z offset so obstacle is centered on prefab
                    navObs.center = new Vector3(0f, navObs.center.y, 0f);


                }

                Vector2Int size = blockDimensions[currentBlockSize];
                buildingCost = size.x * size.y;
                InventoryManager.Instance.RemoveScrap(buildingCost);

                AddBlockToPlaced(clampedGrid, newWall.GetComponent<theWall>());
            }
        }

    }

    public void ToggleGrid()
    {
        gridOverlay.SetActive(!gridOverlay.activeSelf);
        gridMode = !gridMode;
        blockGuide.SetActive(gridMode);
    }

    private bool CanPayForBlock()
    {
        Vector2Int dims = blockDimensions[currentBlockSize];
        int tiles = dims.x * dims.y;

        Debug.Log("Trying to pay for turret. Cost = " + tiles);
        return InventoryManager.Instance.ScrapCount >= tiles;
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

    private Vector2Int GetRotatedBlockDimensions()
    {
        Vector2Int dims = blockDimensions[currentBlockSize];
        // Swap dimensions for odd directions (Left/Right)
        if ((int)currentDirection % 2 == 1)
            return new Vector2Int(dims.y, dims.x);
        return dims;
    }
    private bool DoesBlockOverlap(Vector2Int gridPos)
    {
        Vector2Int rotatedDims = GetRotatedBlockDimensions();

        // Check all cells that this block would occupy
        for (int x = gridPos.x; x < gridPos.x + rotatedDims.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + rotatedDims.y; y++)
            {
                if (placedBlocks.Contains(new Vector2Int(x, y)))
                    return true;
            }
        }
        return false;
    }

    private void AddBlockToPlaced(Vector2Int gridPos, theWall newWall)
    {
        Vector2Int rotatedDims = GetRotatedBlockDimensions();

        for (int x = gridPos.x; x < gridPos.x + rotatedDims.x; x++)
        {
            for (int y = gridPos.y; y < gridPos.y + rotatedDims.y; y++)
            {
                placedBlocks.Add(new Vector2Int(x, y));
                newWall.gridPositions.Add(new Vector2Int(x, y));
            }
        }
    }
}