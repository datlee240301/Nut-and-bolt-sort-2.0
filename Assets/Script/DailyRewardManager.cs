using System;
using TMPro;
using UnityEngine;

public class DailyRewardManager : MonoBehaviour
{
    UiManager uiManager;
    [SerializeField] GameObject greenCheck;

    const string LastClaimDateKey = "DailyReward_LastClaimDate";
    const string ClaimedKey = "DailyReward_Claimed";

    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
        // Kiểm tra ngày mới để reset trạng thái
        string lastDate = PlayerPrefs.GetString(LastClaimDateKey, "");
        string today = DateTime.Now.ToString("yyyyMMdd");
        if (lastDate != today)
        {
            PlayerPrefs.SetInt(ClaimedKey, 0);
            PlayerPrefs.SetString(LastClaimDateKey, today);
            PlayerPrefs.Save();
        }

        // Hiển thị trạng thái
        bool claimed = PlayerPrefs.GetInt(ClaimedKey, 0) == 1;
        greenCheck.SetActive(claimed);
    }

    public void ClaimTicketButton(int amount)
    {
        if (PlayerPrefs.GetInt(ClaimedKey, 0) == 0)
        {
            uiManager.PlusticketNumber(15);
            greenCheck.SetActive(true);

            PlayerPrefs.SetInt(ClaimedKey, 1);
            PlayerPrefs.SetString(LastClaimDateKey, DateTime.Now.ToString("yyyyMMdd"));
            PlayerPrefs.Save();
        }
    }
}