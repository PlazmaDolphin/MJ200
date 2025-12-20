using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridOverlay : MonoBehaviour
{
    public Transform playerPos; // for centering on player
    private int gridSize = 8;
    private float cellSize = 1.25f;
    void Awake()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        int index = 0;
        float z = 0f;

        // Vertical lines
        for (int x = -gridSize; x <= gridSize; x++)
        {
            verts.Add(new Vector3(x * cellSize, -gridSize * cellSize, z));
            verts.Add(new Vector3(x * cellSize,  gridSize * cellSize, z));
            indices.Add(index++);
            indices.Add(index++);
        }

        // Horizontal lines
        for (int y = -gridSize; y <= gridSize; y++)
        {
            verts.Add(new Vector3(-gridSize * cellSize, y * cellSize, z));
            verts.Add(new Vector3( gridSize * cellSize, y * cellSize, z));
            indices.Add(index++);
            indices.Add(index++);
        }

        mesh.SetVertices(verts);
        mesh.SetIndices(indices, MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material =
            new Material(Shader.Find("Sprites/Default"));
    }
    void LateUpdate()
    {
        Vector3 p = playerPos.position;

        float snappedX = Mathf.Floor(p.x / cellSize) * cellSize;
        float snappedY = Mathf.Floor(p.y / cellSize) * cellSize;

        transform.position = new Vector3(snappedX, snappedY, transform.position.z);
    }
}