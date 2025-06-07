using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Image fadeOverlay;
    [SerializeField] GameObject[] levelPrefabs;
    int currentLevel;
    UiManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            SpawnLevel();
            Debug.Log("currentLevel: " + currentLevel);
        }
        else
        {
            PlayerPrefs.SetInt(StringManager.pressLevelButton, 0);
            currentLevel = PlayerPrefs.GetInt(StringManager.currentLevelId, 1);
            PlayerPrefs.SetInt(StringManager.currentLevelId, currentLevel);
            Debug.Log(currentLevel);
        }

        uiManager = FindObjectOfType<UiManager>();
    }

    void SpawnLevel()
    {
        if (PlayerPrefs.GetInt(StringManager.pressLevelButton) == 0)
        {
            int levelId = PlayerPrefs.GetInt(StringManager.currentLevelId);
            if (levelId >= 1 && levelId <= levelPrefabs.Length)
            {
                Instantiate(levelPrefabs[levelId - 1]);
            }
        }
        else
        {
            int levelId = PlayerPrefs.GetInt(StringManager.currentLevelIdLevelButton);
            if (levelId >= 1 && levelId <= levelPrefabs.Length)
            {
                Instantiate(levelPrefabs[levelId - 1]);
            }
        }
    }

    /// Button Zone
    public void LoadLevelButton(int levelId)
    {
        if (levelId <= PlayerPrefs.GetInt(StringManager.currentLevelId))
        {
            PlayerPrefs.SetInt(StringManager.pressLevelButton, 1);
            PlayerPrefs.SetInt(StringManager.currentLevelIdLevelButton, levelId);
            StartCoroutine(FadeAndLoadScene("Main"));
        }
        else
        {
            uiManager.ShowNoticePanel();
        }
    }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        Color color = fadeOverlay.color;
        color.a = 0f;
        fadeOverlay.color = color;
        fadeOverlay.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / duration);
            fadeOverlay.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.color = color;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}