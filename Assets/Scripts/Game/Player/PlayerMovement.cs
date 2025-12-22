using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] // So it automatically gives a rb to the object in the inspector
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D Rb { get; private set; } // Doesn't appears on inspector; Other classes can only read it, not modify;

    public Action OnDashed;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;
    private bool canMove = true;

    private float speed = 5f;


    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            return;
        }

        // Get move input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        
        Rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    public Vector2 GetLastMoveDirection()
    {
        if (moveInput.sqrMagnitude > 0.01f)
            return moveInput.normalized;

        return Vector2.up;
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
