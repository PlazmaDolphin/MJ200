using UnityEngine;

public class ScrapLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        SpawnScrap.RemoveScrap(this.gameObject);
        Destroy(this.gameObject);
    }
}
