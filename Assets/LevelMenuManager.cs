using UnityEngine;
using UnityEngine.UI;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Canvas меню")]
    public GameObject menuCanvas;

    [Header("UI Кнопки и Замки")]
    public Button[] levelButtons;  // 10 кнопок
    public GameObject[] lockIcons; // 9 замков (для уровней 2-10)

    void Start()
    {
        UpdateMenuUI();
    }

    public void UpdateMenuUI()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;

            if (levelNumber <= unlockedLevel)
            {
                levelButtons[i].interactable = true;

                if (i - 1 < lockIcons.Length && i > 0 && lockIcons[i - 1] != null)
                {
                    lockIcons[i - 1].SetActive(false);
                }
            }
            else
            {
                levelButtons[i].interactable = false;

                if (i - 1 < lockIcons.Length && i > 0 && lockIcons[i - 1] != null)
                {
                    lockIcons[i - 1].SetActive(true);
                }
            }
        }
    }

    public void SelectLevel(int levelNumber)
    {
        GameObject pig = GameObject.Find("Бешеная свинья");
        Camera cam = Camera.main;
        GameObject spawnPoint = GameObject.Find("spawn_" + levelNumber);

        if (pig != null && spawnPoint != null)
        {
            // 1. Телепортируем свинью
            pig.transform.position = spawnPoint.transform.position;

            // 2. ОБНОВЛЯЕМ ТОЧКУ РЕСПАУНА ДЛЯ ЭТОГО УРОВНЯ!
            PigController pigController = pig.GetComponent<PigController>();
            if (pigController != null)
            {
                pigController.SetRespawnPoint(spawnPoint.transform.position);
            }

            // 3. Сбрасываем скорость
            Rigidbody2D rb = pig.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 4. Перемещаем камеру
            if (cam != null)
            {
                cam.transform.position = new Vector3(pig.transform.position.x, pig.transform.position.y + 1.5f, -10f);
            }

            // 5. Закрываем меню
            if (menuCanvas != null)
            {
                menuCanvas.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Не удалось найти Свинью или Спавн 'spawn_" + levelNumber + "'!");
        }
    }

    public void ShowMenu()
    {
        if (menuCanvas != null) menuCanvas.SetActive(true);
        UpdateMenuUI();
    }
}