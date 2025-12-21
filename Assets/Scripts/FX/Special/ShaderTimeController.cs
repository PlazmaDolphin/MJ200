using System.Collections.Generic;
using UnityEngine;

public class ShaderTimeController : MonoBehaviourSingleton<ShaderTimeController>
{
    public List<Material> materials; // Assign the material using your shader
    private float unscaledTime;

    void Update()
    {
        // Accumulate unscaled time
        unscaledTime += Time.unscaledDeltaTime;

        SetTime();
    }

    private void SetTime()
    {
        foreach (var item in materials)
        {
            // Pass the unscaled time to the shader
            item.SetFloat("_UnscaledTime", unscaledTime);
        }
    }

    private void OnDisable()
    {
        unscaledTime = 0;
        SetTime();
    }
}
