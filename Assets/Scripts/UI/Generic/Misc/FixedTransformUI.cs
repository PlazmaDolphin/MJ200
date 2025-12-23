using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedTransformUI : MonoBehaviour
{
    private Vector3 lockedScale;  // The fixed scale for the UI element
    private Quaternion lockedRotation;  // The fixed rotation for the UI element (Euler angles)

    private RectTransform rectTransform;

    private void Awake()
    {
        lockedScale = transform.localScale;
        lockedRotation = transform.localRotation;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // Lock scale and rotation
        rectTransform.localScale = lockedScale;
        rectTransform.rotation = lockedRotation;
    }
}
