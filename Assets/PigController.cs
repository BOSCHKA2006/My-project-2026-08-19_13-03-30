using UnityEngine;

public class PigController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 13f;
    public float jumpForce = 10f;
    public float bounceForce = 8f; // Сила отскока от головы сосиски

    [Header("Проверка земли")]
    public float rayDistance = 2.28f;

    [Header("Падение за сцену")]
    public float fallLimitY = -10f;

    private Rigidbody2D rb;
    private Animator animator;
    private float moveInput;
    private bool isGrounded;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (transform.position.y < fallLimitY)
        {
            Respawn();
        }

        moveInput = Input.GetAxisRaw("Horizontal");

        // Проверка земли
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, rayDistance);
        isGrounded = false;

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != gameObject && hit.collider.CompareTag("Ground"))
            {
                isGrounded = true;
                break;
            }
        }

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Анимации
        bool isMoving = moveInput != 0;
        animator.SetBool("isWalking", isMoving);
        animator.SetBool("isGrounded", isGrounded);

        // Разворот
        if (moveInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (isGrounded && !Input.GetKey(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    public void Respawn()
    {
        transform.position = startPosition;
        rb.linearVelocity = Vector2.zero;
    }

    // Обработка взаимодействия со всеми опасностями
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHazard(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHazard(collision.collider);
    }

    private void HandleHazard(Collider2D col)
    {
        // 1. Попали в огонь — смерть
        if (col.CompareTag("Fire"))
        {
            Respawn();
            return;
        }

        // 2. Коснулись сосиски
        if (col.CompareTag("Enemy"))
        {
            SausageEnemy sausage = col.GetComponent<SausageEnemy>();
            if (sausage != null && sausage.isDead) return;

            // Проверяем: свинья находится выше центра сосиски И падает вниз
            bool isLandingOnHead = transform.position.y > col.bounds.center.y + 0.2f && rb.linearVelocity.y <= 1f;

            if (isLandingOnHead)
            {
                // Сплющиваем сосиску!
                if (sausage != null) sausage.Squash();

                // Свинья смачно подпрыгивает вверх
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
            }
            else
            {
                // Удар сбоку — свинья проиграла
                Respawn();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.down * rayDistance);
    }

    public void SetRespawnPoint(Vector3 newPoint)
    {
        startPosition = newPoint;
    }
}