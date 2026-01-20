using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class RandomNPCWalker : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float directionChangeInterval = 3f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float directionTimer;

    private Vector3 verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        PickNewDirection();
    }

    private void Update()
    {
        // Gravity
        if (!controller.isGrounded)
        {
            verticalVelocity.y += gravity * Time.deltaTime;
        }
        else
        {
            verticalVelocity.y = 0f;
        }

        // Timer to change direction
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0f)
        {
            PickNewDirection();
        }

        // Rotate towards movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // Move
        Vector3 velocity = moveDirection * moveSpeed + verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    private void PickNewDirection()
    {
        Vector2 random2D = Random.insideUnitCircle.normalized;
        moveDirection = new Vector3(random2D.x, 0f, random2D.y);
        directionTimer = directionChangeInterval;
    }
}

