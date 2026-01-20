using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public enum typeAttack
{
    Normal,
    Special,
    none
}

public class Moveset : MonoBehaviour
{
    // Variabili per movimento
    public float moveSpeed = 3f;
    public float runSpeed = 8f;

    private float targetSpeed;
    private float currentSpeed;

    private const float moveSpeedMultipler = 0.5f;
    public bool isRunning;

    // Variabili per attacchi
    public float attackCooldown = 0.3f;
    private bool canAttack = true;

    // VARIABILE FONDAMENTALE PER IL BLOCCO DEL MOVIMENTO
    public bool isMovementBlocked = false;

    [SerializeField] private PlayerStats stats;
    [SerializeField] private AudioController audioController;

    // Riferimenti a componenti
    private Rigidbody rb;
    private Animator animator;
    private float speed;

    public Camera mainCamera;

    public void MovesetIni(PlayerStats stats, Camera camera)
    {
        this.stats = stats;
        this.mainCamera = camera;
    }

    public void Awake()
    {
        if (audioController == null)
        {
            audioController = FindObjectOfType<AudioController>();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentSpeed = moveSpeed;
        targetSpeed = moveSpeed;
    }

    public void footStep()
    {
        audioController.playFootstep();
    }

    public void moveSet(float horizontalInput, float verticalInput, bool shiftPressed = false)
    {
        if (isMovementBlocked)
        {
            stopMovement();
            return;
        }

        isRunning = shiftPressed && stats.getStamina() != 0.0f;
        targetSpeed = isRunning ? runSpeed : moveSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / moveSpeedMultipler);

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 movementDirection = (forward * verticalInput + right * horizontalInput).normalized;

        if (movementDirection.magnitude >= 0.1f)
        {
            // Usiamo un Raycast per determinare la pendenza del terreno
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, 1.0f))
            {
                // Proiettiamo il vettore di movimento sul piano inclinato del terreno
                Vector3 projectedMove = Vector3.ProjectOnPlane(movementDirection, hitInfo.normal);
                rb.velocity = projectedMove * currentSpeed;
            }
            else
            {
                // Se non siamo a terra, usiamo la logica precedente per la gravità
                Vector3 verticalVelocity = new Vector3(0, rb.velocity.y, 0);
                Vector3 horizontalVelocity = movementDirection * currentSpeed;
                rb.velocity = horizontalVelocity + verticalVelocity;
            }

            speed = moveSpeed;
        }
        else
        {
            speed = 0;
            // Mantieni la velocità verticale per la gravità
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        itsRun();
    }

    public void stopMovement()
    {
        speed = 0;
        rb.velocity = Vector3.zero;
    }

    public void rotazione(float horizontalInput, float verticalInput)
    {
        if (isMovementBlocked) return;

        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 movementDirection = (forward * verticalInput + right * horizontalInput).normalized;
        if (movementDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10.0f);
        }
    }

    public void itsRun()
    {
        animator.SetFloat("Speed", speed);
        animator.SetBool("isRunning", isRunning);
    }

    public void inputAttack(typeAttack tipo = typeAttack.none)
    {
        if (isMovementBlocked) return;

        if (!checkStamina(tipo)) return;

        if (tipo == typeAttack.Special)
        {
            Attack(true);
        }
        else if (tipo == typeAttack.Normal)
        {
            Attack();
        }
    }

    public bool checkStamina(typeAttack tipo)
    {
        if (tipo == typeAttack.Special && stats.getStamina() >= 20f)
        {
            return true;
        }
        else if (tipo == typeAttack.Normal && stats.getStamina() >= 10f)
        {
            return true;
        }
        return false;
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void Attack(bool special = false)
    {
        stats.callUseStamina(special ? 20f : 10f);
        stats.resetTimer();
        if (canAttack)
        {
            if (special)
                specialAttack();
            else
                animator.SetTrigger("Attack");

            StartCoroutine(AttackCooldown());
        }
    }

    void specialAttack()
    {
        animator.SetTrigger("SpecialAttack");
    }

    public void setStats(PlayerStats stats)
    {
        this.stats = stats;
    }

    public void setCamera(Camera camera)
    {
        this.mainCamera = camera;
    }

    public PlayerStats getStats()
    {
        return this.stats;
    }

    public Camera getCamera()
    {
        return this.mainCamera;
    }
}