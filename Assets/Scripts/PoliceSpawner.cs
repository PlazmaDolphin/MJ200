using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    public GameObject policePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if there are no police left, spawn a new one
        if (GameObject.FindGameObjectsWithTag("police").Length == 0)
        {
            Instantiate(policePrefab, transform.position, Quaternion.identity);
        }
    }
}
