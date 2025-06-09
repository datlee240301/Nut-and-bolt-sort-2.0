using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    Image levelButtonImage;
    [SerializeField] Sprite finishSprite;
    [SerializeField] Sprite notFinishSprite;

    [SerializeField] int buttonId;
    int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        levelButtonImage = GetComponent<Image>();
        SetButtonImage();
    }

    public void SetButtonImage()
    {
        //currentLevel = PlayerPrefs.GetInt(StringManager.currentLevelId, 1);
        if (buttonId <= PlayerPrefs.GetInt(StringManager.currentLevelId))
        {
            levelButtonImage.sprite = notFinishSprite;
        }
        else
        {
            levelButtonImage.sprite = finishSprite;
        }
    }
}