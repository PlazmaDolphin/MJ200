using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerVisuals : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;

    private Transform parentTransform;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer sprite;

    [Space(20f), Header("Animations")]
    [SerializeField] private AnimationClip idle;
    [SerializeField] private AnimationClip walk;
    [SerializeField] private AnimationClip build;
    [SerializeField] private AnimationClip salvaging;

    //[SerializeField] private AnimationClip shoot;
    //[SerializeField] private AnimationClip dash;
    private float actionLockTimer = 0f;

    private bool isBuilding, isSalvaging;

    private float initXScale;

    private void Start()
    {
        parentTransform = transform.parent;
        controller = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();

        initXScale = parentTransform.localScale.x;

        controller.OnBuildingStateChanged += OnBuildingStateChanged;
        controller.OnSalvagingStateChanged += OnSalvagingStateChanged;
        //manager.Controlller.OnShoot += OnShoot;
        //manager.Movement.OnDashed += OnDashed;
        //manager.Health.OnDamaged += OnDamaged;
    }

    private void OnSalvagingStateChanged(bool isSalvaging)
    {
        this.isSalvaging = isSalvaging;

        if(isSalvaging)
        {
            animator.Play(salvaging.name);
        }
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
        if (actionLockTimer > 0f || isBuilding || isSalvaging)
            return;

        Vector2 moveInput = controller.Movement.MoveInput;

        if (moveInput.magnitude > 0.1f)
        {
            animator.Play(walk.name);
            
        }
        else
        {
            animator.Play(idle.name);
        }

        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        mouseScreen.z = Camera.main.nearClipPlane;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        if (mouseWorld.x < transform.position.x)
        {
            parentTransform.localScale = new Vector3(-Mathf.Abs(initXScale), parentTransform.localScale.y, parentTransform.localScale.z);
        }
        else
        {
            parentTransform.localScale = new Vector3(Mathf.Abs(initXScale), parentTransform.localScale.y, parentTransform.localScale.z);
        }

        //parentTransform.localScale = new Vector3(initXScale * (Input.mousePosition.x < Screen.width / 2 ? -1 : 1), parentTransform.localScale.y, parentTransform.localScale.z);

    }

    //private void PlayAction(AnimationClip clip)
    //{
    //    if (clip == null) return;

    //    animator.Play(clip.name);
    //    actionLockTimer = clip.length;
    //}
}