using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [Header("Настройки финиша")]
    public int nextLevelToUnlock = 2; // Какой уровень разблокировать (для 1 уровня это 2)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем касание свиньи
        if (collision.CompareTag("Player") || collision.gameObject.name.Contains("Бешеная свинья") || collision.transform.root.name.Contains("Бешеная свинья"))
        {
            // 1. Сохраняем открытый уровень в память
            int currentUnlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
            if (nextLevelToUnlock > currentUnlocked)
            {
                PlayerPrefs.SetInt("UnlockedLevel", nextLevelToUnlock);
                PlayerPrefs.Save();
            }

            // 2. Ищем Менеджер Меню (ДАЖЕ ЕСЛИ ОН ВЫКЛЮЧЕН!)
            LevelMenuManager menuManager = FindAnyObjectByType<LevelMenuManager>(FindObjectsInactive.Include);

            if (menuManager != null)
            {
                menuManager.ShowMenu(); // Включаем меню!
            }
            else
            {
                Debug.LogError("Не удалось найти LevelMenuManager на сцене!");
            }
        }
    }
}