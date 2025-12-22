using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer sprite;

    [Space(20f), Header("Animations")]
    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip build;

    //[SerializeField] private AnimationClip shoot;
    //[SerializeField] private AnimationClip dash;
    private float actionLockTimer = 0f;

    private bool isBuilding;

    private void Start()
    {
        controller = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();

        controller.OnBuildingStateChanged += OnBuildingStateChanged;
        //manager.Controlller.OnShoot += OnShoot;
        //manager.Movement.OnDashed += OnDashed;
        //manager.Health.OnDamaged += OnDamaged;
    }

    private void OnBuildingStateChanged(bool isBuilding)
    {
        this.isBuilding = isBuilding;

        if (isBuilding)
        {
            animator.Play(build.name);
        }
    }

    private void Update()
    {
        if (actionLockTimer > 0f)
            actionLockTimer -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (actionLockTimer > 0f || isBuilding)
            return;

        Vector2 moveInput = controller.Movement.MoveInput;

        if (moveInput.magnitude > 0.1f)
        {
            animator.Play(walk.name);
            sprite.flipX = moveInput.x < 0;
        }
        else
        {
            animator.Play(idle.name);
        }
    }

    //private void PlayAction(AnimationClip clip)
    //{
    //    if (clip == null) return;

    //    animator.Play(clip.name);
    //    actionLockTimer = clip.length;
    //}
}