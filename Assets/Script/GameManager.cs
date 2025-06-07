using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] UiPanelDotween winPanel;
    [SerializeField] GameObject[] winEffects;
    UiManager uiManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        uiManager = FindObjectOfType<UiManager>();
    }

    public void CheckWinCondition()
    {
        TubeController[] allTubes = FindObjectsOfType<TubeController>();
        foreach (TubeController tube in allTubes)
        {
            var nuts = tube.GetCurrentNuts();
            if (nuts.Count == 0) continue;
            if (nuts.Count != 4) return;
            string tagCheck = nuts[0].tag;
            bool allSame = nuts.TrueForAll(nut => nut.tag == tagCheck);
            if (!allSame) return;
        }

        if (PlayerPrefs.GetInt(StringManager.pressLevelButton) == 0)
        {
            winPanel.PanelFadeIn();
            int currentLevel = PlayerPrefs.GetInt(StringManager.currentLevelId);
            PlayerPrefs.SetInt(StringManager.currentLevelId, currentLevel + 1);
        }
        else
        {
            winPanel.PanelFadeIn();
            if (PlayerPrefs.GetInt(StringManager.currentLevelIdLevelButton) ==
                PlayerPrefs.GetInt(StringManager.currentLevelId) - 1 ||
                PlayerPrefs.GetInt(StringManager.currentLevelIdLevelButton) ==
                PlayerPrefs.GetInt(StringManager.currentLevelId))
                PlayerPrefs.SetInt(StringManager.currentLevelId, PlayerPrefs.GetInt(StringManager.currentLevelId) + 1);
        }

        foreach (GameObject go in winEffects)
        {
            if (go != null)
            {
                go.SetActive(true);
            }
        }
        uiManager.PlusticketNumber(10);
    }
}