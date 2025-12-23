using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    Camera cam;

    void Awake()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (!cam) return;
        transform.forward = cam.transform.forward; // altijd "vlak" naar camera
    }
}