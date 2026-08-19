using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Ссылка на свинью
    public float smoothSpeed = 5f;  // Плавность слежения
    public Vector3 offset = new Vector3(0, 1.5f, -10f); // Смещение (Z обязательно -10!)

    void LateUpdate()
    {
        if (target == null) return;

        // Рассчитываем положение камеры с учетом смещения
        Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);

        // Плавный занос камеры за персонажем
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}