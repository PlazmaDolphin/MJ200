using UnityEngine;

public class FixedTransform : MonoBehaviour
{
    private Vector3 lockedScale;  // The fixed scale for the UI element
    private Quaternion lockedRotation;  // The fixed rotation for the UI element (Euler angles)

    private void Awake()
    {
        lockedScale = transform.localScale;
        lockedRotation = transform.localRotation;
    }

    private void LateUpdate()
    {
        // Lock scale and rotation
        transform.localScale = lockedScale;
        transform.rotation = lockedRotation;
    }
}
