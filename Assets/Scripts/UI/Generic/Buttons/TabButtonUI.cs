using UnityEngine;

public class TabButtonUI : MonoBehaviour
{
    public BaseButtonUI Button { get; private set; }

    [Header("Clips")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip selectedClip;
    [SerializeField] private AnimationClip idle;

    private void Awake()
    {
        Button = GetComponent<BaseButtonUI>();
    }

    public void SetState(bool selected)
    {
        if (animator)
        {
            AnimationClip clip = selected ? selectedClip : idle;
            animator.Play(clip.name);
        }
    }
}
