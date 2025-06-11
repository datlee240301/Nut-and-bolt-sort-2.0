using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] int themePrice;
    [SerializeField] GameObject[] buyThemeButtons;
    [SerializeField] GameObject[] greyPanel;
    [SerializeField] GameObject[] lockIcons;
    [SerializeField] GameObject[] greenCheckIcons;
    [SerializeField] TextMeshProUGUI ticketNumberText;
    [SerializeField] Sprite[] themePrefabs;
    [SerializeField] Image backgroundSpriteRenderer;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            InitDefaultTheme();
            SetBuyStatus();
            SetGreenCheckStatus();
        }
        else
        {
            SetBackground();
        }
    }

    void InitDefaultTheme()
    {
        // Nếu chưa từng lưu trạng thái theme 1, set mặc định đã mua và đang dùng
        if (!PlayerPrefs.HasKey(StringManager.GetThemeKey(1)))
        {
            PlayerPrefs.SetInt(StringManager.GetThemeKey(1), 1); // Đã mua theme 1
            PlayerPrefs.SetInt(StringManager.useTheme1, 1);      // Đang dùng theme 1
            PlayerPrefs.Save();
        }
    }

    void SetBuyStatus()
    {
        for (int i = 0; i < buyThemeButtons.Length; i++)
        {
            if (PlayerPrefs.GetInt(StringManager.GetThemeKey(i + 1), 0) == 1)
            {
                buyThemeButtons[i].SetActive(false);
                greyPanel[i].SetActive(false);
                lockIcons[i].SetActive(false);
            }
        }
    }

    void SetGreenCheckStatus()
    {
        for (int i = 0; i < greenCheckIcons.Length; i++)
        {
            greenCheckIcons[i].SetActive(PlayerPrefs.GetInt($"UseTheme{i + 1}") == 1);
        }
    }

    void SetBackground()
    {
        for (int i = 0; i < themePrefabs.Length; i++)
        {
            if (PlayerPrefs.GetInt($"UseTheme{i + 1}") == 1)
            {
                backgroundSpriteRenderer.sprite = themePrefabs[i];
                break;
            }
        }
    }

    /// <summary>
    /// Button Zone
    /// </summary>
    public void BuyThemeButton(int themeId)
    {
        if (themeId == 1) return; // Không cho mua theme mặc định

        int ticketNumber = PlayerPrefs.GetInt(StringManager.ticketNumber, 0);
        if (ticketNumber >= themePrice)
        {
            int index = themeId - 1;
            if (index >= 0 && index < buyThemeButtons.Length)
            {
                buyThemeButtons[index].SetActive(false);
                greyPanel[index].SetActive(false);
                lockIcons[index].SetActive(false);
                PlayerPrefs.SetInt(StringManager.GetThemeKey(themeId), 1);
                PlayerPrefs.SetInt(StringManager.ticketNumber, ticketNumber - themePrice);
                if (ticketNumberText != null)
                {
                    ticketNumberText.text = PlayerPrefs.GetInt(StringManager.ticketNumber).ToString();
                }
            }
        }
    }

    public void UseThemeButton(int themeId)
    {
        if (themeId < 1 || themeId > greenCheckIcons.Length)
            return;

        // Reset tất cả useTheme về 0
        for (int i = 1; i <= greenCheckIcons.Length; i++)
        {
            PlayerPrefs.SetInt($"UseTheme{i}", 0);
        }

        // Set theme được chọn về 1
        PlayerPrefs.SetInt($"UseTheme{themeId}", 1);
        PlayerPrefs.Save();

        for (int i = 0; i < greenCheckIcons.Length; i++)
        {
            greenCheckIcons[i].SetActive(i == (themeId - 1));
        }
    }
}