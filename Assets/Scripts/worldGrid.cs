using UnityEngine;

public class worldGrid : MonoBehaviour
{
    public GameObject gridOverlay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        //toggle grid on/off with E key
        if (Input.GetKeyDown(KeyCode.E)) {
            gridOverlay.SetActive(!gridOverlay.activeSelf);
        }
    }
}
