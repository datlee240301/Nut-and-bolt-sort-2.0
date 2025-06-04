using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("----------FadeZone-----------")] [SerializeField]
    Image fadeOverlay;

    [Header("----------TextZone-----------")] [SerializeField]
    TextMeshProUGUI ticketNumberText;

    int ticketNumber;
    [SerializeField] TextMeshProUGUI undoNumberText;
    int undoNumber;
    [SerializeField] TextMeshProUGUI addTubeNumberText;
    int addTubeNumber;

    [Header("----------SettingButtonZone-----------")] [SerializeField]
    Image musicButton;

    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;
    [SerializeField] Image soundButton;
    [SerializeField] Sprite soundOnSprite;
    [SerializeField] Sprite soundOffSprite;
    [SerializeField] Image vibrateButton;
    [SerializeField] Sprite vibrateOnSprite;
    [SerializeField] Sprite vibrateOffSprite;
    int musicId;
    int soundId;
    int vibrateId;

    [Header("----------BuyThemeButtonZone-----------")] [SerializeField]
    GameObject[] buyThemeButtons;
    [SerializeField] GameObject[] greyPanel;
    [Header("----------ObjectToHide-----------")] [SerializeField] 
    GameObject[] objectsToHide;
    // Start is called before the first frame update
    void Start()
    {
        SetTicketNumber();
        SetSettingButton();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// Function zone
    /// </summary>
    void SetTicketNumber()
    {
        ticketNumber = PlayerPrefs.GetInt("TicketNumber", 2);
        PlayerPrefs.SetInt(StringManager.ticketNumber, ticketNumber);
        undoNumber = PlayerPrefs.GetInt(StringManager.undoNumber, 3);
        PlayerPrefs.SetInt(StringManager.undoNumber, undoNumber);
        addTubeNumber = PlayerPrefs.GetInt(StringManager.addTubeNumber, 2);
        PlayerPrefs.SetInt(StringManager.addTubeNumber, addTubeNumber);

        ticketNumber = PlayerPrefs.GetInt(StringManager.ticketNumber);
        if (ticketNumberText != null)
            ticketNumberText.text = ticketNumber.ToString();
        undoNumber = PlayerPrefs.GetInt(StringManager.undoNumber);
        if (undoNumberText != null)
            undoNumberText.text = undoNumber.ToString();
        addTubeNumber = PlayerPrefs.GetInt(StringManager.addTubeNumber);
        if (addTubeNumberText != null)
            addTubeNumberText.text = addTubeNumber.ToString();
    }

    public void MinusUndoNumber(int number)
    {
        undoNumber -= number;
        if (undoNumber < 0)
            undoNumber = 0;
        PlayerPrefs.SetInt(StringManager.undoNumber, undoNumber);
        if (undoNumberText != null)
            undoNumberText.text = undoNumber.ToString();
    }

    public void MinusAddTubeNumber(int number)
    {
        addTubeNumber -= number;
        if (addTubeNumber < 0)
            addTubeNumber = 0;
        PlayerPrefs.SetInt(StringManager.addTubeNumber, addTubeNumber);
        if (addTubeNumberText != null)
            addTubeNumberText.text = addTubeNumber.ToString();
    }

    void SetSettingButton()
    {
        if (musicButton != null)
        {
            musicId = PlayerPrefs.GetInt(StringManager.musicId, 1);
            PlayerPrefs.SetInt(StringManager.musicId, musicId);
            musicButton.sprite = musicId == 1 ? musicOnSprite : musicOffSprite;
        }

        if (soundButton != null)
        {
            soundId = PlayerPrefs.GetInt(StringManager.soundId, 1);
            PlayerPrefs.SetInt(StringManager.soundId, soundId);
            soundButton.sprite = soundId == 1 ? soundOnSprite : soundOffSprite;
        }

        if (vibrateButton != null)
        {
            vibrateId = PlayerPrefs.GetInt(StringManager.vibrateID, 1);
            PlayerPrefs.SetInt(StringManager.vibrateID, vibrateId);
            vibrateButton.sprite = vibrateId == 1 ? vibrateOnSprite : vibrateOffSprite;
        }
    }

    /// <summary>
    /// Button zone
    ///<summary>
    public void HideObjects()
    {
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(false);
        }
    }
    public void ShowObjects()
    {
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(true);
        }
    }
    public void BuyTicket(int amount)
    {
        ticketNumber += amount;
        PlayerPrefs.SetInt(StringManager.ticketNumber, ticketNumber);
        SetTicketNumber();
    }

    public void MusicButton()
    {
        if (musicButton.sprite == musicOnSprite)
        {
            musicButton.sprite = musicOffSprite;
            PlayerPrefs.SetInt(StringManager.musicId, 0);
        }
        else
        {
            musicButton.sprite = musicOnSprite;
            PlayerPrefs.SetInt(StringManager.musicId, 1);
        }
    }

    public void SoundButton()
    {
        if (soundButton.sprite == soundOnSprite)
        {
            soundButton.sprite = soundOffSprite;
            PlayerPrefs.SetInt(StringManager.soundId, 0);
        }
        else
        {
            soundButton.sprite = soundOnSprite;
            PlayerPrefs.SetInt(StringManager.soundId, 1);
        }
    }

    public void VibrateButton()
    {
        if (vibrateButton.sprite == vibrateOnSprite)
        {
            vibrateButton.sprite = vibrateOffSprite;
            PlayerPrefs.SetInt(StringManager.vibrateID, 0);
        }
        else
        {
            vibrateButton.sprite = vibrateOnSprite;
            PlayerPrefs.SetInt(StringManager.vibrateID, 1);
            PlayerPrefs.SetInt(StringManager.vibrateID, 1);
        }
    }

    public void LoadSceneButton(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
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

    public void BuyThemeButton(int price)
    {
        if (ticketNumber >= price)
        {
            ticketNumber -= price;
            PlayerPrefs.SetInt(StringManager.ticketNumber, ticketNumber);
            SetTicketNumber();
        }
    }

    public void SetThemeIdButton(int themeId)
    {
        if (themeId == 1)
        {
            PlayerPrefs.SetInt(StringManager.hasBuyTheme1,1);
            buyThemeButtons[0].SetActive(false);
        }
        else if (themeId == 2)
        {
            PlayerPrefs.SetInt(StringManager.hasBuyTheme2,1);
            buyThemeButtons[1].SetActive(false);
        }
        else if (themeId == 3)
        {
            PlayerPrefs.SetInt(StringManager.hasBuyTheme3,1);
            buyThemeButtons[2].SetActive(false);
        }
        else if (themeId == 4)
        {
            PlayerPrefs.SetInt(StringManager.hasBuyTheme4,1);
            buyThemeButtons[3].SetActive(false);
        }
        else if (themeId == 5)
        {
            PlayerPrefs.SetInt(StringManager.hasBuyTheme5,1);
            buyThemeButtons[4].SetActive(false);
        }
        else if (themeId == 6)
        {
            PlayerPrefs.SetInt(StringManager.hasBuyTheme6,1);
            buyThemeButtons[5].SetActive(false);
        }
    }
}