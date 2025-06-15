using TMPro;
using UnityEngine;

public class FakeStoreManager : MonoBehaviour
{
    private readonly string[] packs =
    {
        "100coin.diamondsort.pack1",
        "300coin.diamondsort.pack2",
        "500coin.diamondsort.pack3",
        "900coin.diamondsort.pack4",
        "1200coin.diamondsort.pack5",
        "1500coin.diamondsort.pack6",
        "2000coin.diamondsort.pack7",
        "2500coin.diamondsort.pack8",
        "3000coin.diamondsort.pack9"
    };

    UiManager uiManager;
    [SerializeField] private TextMeshProUGUI doYouText;

    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
    }

    public void UpdateText()
    {
        if (doYouText == null) return;

        int packId = PlayerPrefs.GetInt(StringManager.PackId, 0);
        if (packId >= 1 && packId <= packs.Length)
        {
            doYouText.text = $"Do you want to Purchase {packs[packId - 1]}?";
        }
        else
        {
            doYouText.text = "Invalid pack selected.";
        }
    }

    public void BuyCoinButton()
    {
        int packId = PlayerPrefs.GetInt(StringManager.PackId);
        int[] ticketAmounts = { 100, 300, 500, 900, 1200, 1500, 2000, 2500, 3000 };
        if (packId >= 1 && packId <= ticketAmounts.Length)
        {
            uiManager.PlusticketNumber(ticketAmounts[packId - 1]);
        }
    }
}