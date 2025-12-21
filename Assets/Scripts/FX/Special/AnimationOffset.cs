using UnityEngine;

public class AnimationOffset : MonoBehaviour
{
    [SerializeField] private string cycleOffset = "Cycle Offset";
    [SerializeField][Range(0f, 1f)] private float maxOffset = 1f;

    private void Awake()
    {
        if (!TryGetComponent(out Animator animator))
        {
            Debug.LogError($"{gameObject.name} doesn't has animator component so can't randomize the offset");
            return;
        }

        if (!string.IsNullOrEmpty(cycleOffset))
        {
            float randomCycleOffset = Random.Range(0f, maxOffset);
            //Debug.Log("random animation offset" + randomValue);
            animator.SetFloat(cycleOffset, randomCycleOffset);
        }
    }
}
