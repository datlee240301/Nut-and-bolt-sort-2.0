using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeManager : MonoBehaviour
{
    [SerializeField] int themePrice;
    [SerializeField] GameObject[] buyThemeButtons;
    [SerializeField] GameObject[] greyPanel;
    [SerializeField] GameObject[] lockIcons;
    [SerializeField] GameObject[] greenCheckIcons;
    [SerializeField] TextMeshProUGUI ticketNumberText;
    [SerializeField] Sprite[] themePrefabs;
    [SerializeField] SpriteRenderer backgroundSpriteRenderer;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            SetBuyStatus();
            SetGreenCheckStatus();
            SetGreenCheckStatus();
        }
        else
        {
            SetBackground();
            Debug.Log(PlayerPrefs.GetInt(StringManager.useTheme2));
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
        for (int i = 0; i < 6; i++)
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

        // Reset all useTheme về 0
        PlayerPrefs.SetInt(StringManager.useTheme1, 0);
        PlayerPrefs.SetInt(StringManager.useTheme2, 0);
        PlayerPrefs.SetInt(StringManager.useTheme3, 0);
        PlayerPrefs.SetInt(StringManager.useTheme4, 0);
        PlayerPrefs.SetInt(StringManager.useTheme5, 0);
        PlayerPrefs.SetInt(StringManager.useTheme6, 0);

        // Set theme được chọn về 1
        switch (themeId)
        {
            case 1: PlayerPrefs.SetInt(StringManager.useTheme1, 1); break;
            case 2: PlayerPrefs.SetInt(StringManager.useTheme2, 1); break;
            case 3: PlayerPrefs.SetInt(StringManager.useTheme3, 1); break;
            case 4: PlayerPrefs.SetInt(StringManager.useTheme4, 1); break;
            case 5: PlayerPrefs.SetInt(StringManager.useTheme5, 1); break;
            case 6: PlayerPrefs.SetInt(StringManager.useTheme6, 1); break;
        }

        PlayerPrefs.Save();

        foreach (var greenCheck in greenCheckIcons)
        {
            greenCheck.SetActive(false);
        }
        greenCheckIcons[themeId - 1].SetActive(true);
    }
}