using UnityEngine;

public class SausageEnemy : MonoBehaviour
{
    [Header("Точки движения")]
    public Transform pointA;
    public Transform pointB;

    [Header("Настройки ходьбы")]
    public float speed = 2f;

    [Header("Настройки покачивания")]
    public float wobbleSpeed = 8f;
    public float wobbleAngle = 12f;

    [Header("Звук сплющивания")]
    public AudioClip squishSound; // Сюда перетащите аудиофайл

    [HideInInspector]
    public bool isDead = false;

    private Transform currentTarget;
    private Animator animator;
    private Collider2D col;

    void Start()
    {
        currentTarget = pointB;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // Если сосиска раздавлена — перестаем двигаться и качаться
        if (isDead || pointA == null || pointB == null) return;

        // 1. Движение только по X
        Vector2 targetPos = new Vector2(currentTarget.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 2. Покачивание
        float zAngle = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAngle;
        transform.rotation = Quaternion.Euler(0, 0, zAngle);

        // 3. Разворот
        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.05f)
        {
            if (currentTarget == pointB)
            {
                currentTarget = pointA;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                currentTarget = pointB;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    // Метод сплющивания сосиски
    public void Squash()
    {
        if (isDead) return;
        isDead = true;

        // Сбрасываем наклон сосиски в ровное положение
        transform.rotation = Quaternion.identity;

        // Воспроизводим звук
        if (squishSound != null)
        {
            AudioSource.PlayClipAtPoint(squishSound, transform.position);
        }

        // Запускаем анимацию сплющивания
        if (animator != null)
        {
            animator.Play("SOSISON");
        }

        // Отключаем коллайдер, чтобы свинья не спотыкалась об мертвую сосиску
        if (col != null) col.enabled = false;

        // Удаляем объект со стола через 1.5 секунды после окончания анимации
        Destroy(gameObject, 1.5f);
    }
}