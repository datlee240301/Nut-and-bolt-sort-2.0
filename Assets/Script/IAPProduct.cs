using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MyPurchaseID
{
    // public const string RemoveAds = "com.minigamehub.removeads";
}

public class IAPProduct : MonoBehaviour
{
    [SerializeField] private string _purchaseID;
    [SerializeField] private Button _purchaseButton;
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private TextMeshProUGUI _discount;
    [SerializeField] private Sprite _icon;

    public string PurchaseID => _purchaseID;

    public delegate void PurchaseEvent(Product Model, Action OnComplete);

    public event PurchaseEvent OnPurchase;
    private Product _model;
    IAPManager iapManager;
    UiManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
        iapManager = FindObjectOfType<IAPManager>();
        RegisterPurchase();
        RegisterEventButton();
    }

    protected virtual void RegisterPurchase()
    {
        StartCoroutine(iapManager.CreateHandleProduct(this));
    }

    public void Setup(Product product, string code, string price)
    {
        _model = product;
        if (_price != null)
        {
            _price.text = price + " " + code;
        }

        if (_discount != null)
        {
            if (code.Equals("VND"))
            {
                var round = Mathf.Round(float.Parse(price) + float.Parse(price) * .4f);
                _discount.text = code + " " + round;
            }
            else
            {
                var priceFormat = $"{float.Parse(price) + float.Parse(price) * .4f:0.00}";
                _discount.text = code + " " + priceFormat;
            }
        }
    }

    private void RegisterEventButton()
    {
        _purchaseButton.onClick.AddListener(() =>
        {
            //AudioManager.PlaySound("Click");
            Purchase();
        });
    }

    private void Purchase()
    {
        OnPurchase?.Invoke(_model, HandlePurchaseComplete);
    }

    private void HandlePurchaseComplete()
    {
        switch (_purchaseID)
        {
            case "100coin.diamondsort.pack1":
                uiManager.PlusticketNumber(100);
                break;
            case "300coin.diamondsort.pack2":
                uiManager.PlusticketNumber(300);
                break;
            case "500coin.diamondsort.pack3":
                uiManager.PlusticketNumber(500);
                break;
            case "900coin.diamondsort.pack4":
                uiManager.PlusticketNumber(900);
                break;
            case "1200coin.diamondsort.pack5":
                uiManager.PlusticketNumber(1200);
                break;
            case "1500coin.diamondsort.pack6":
                uiManager.PlusticketNumber(1500);
                break;
            case "2000coin.diamondsort.pack7":
                uiManager.PlusticketNumber(2000);
                break;
            case "2500coin.diamondsort.pack8":
                uiManager.PlusticketNumber(2500);
                break;
            case "3000coin.diamondsort.pack9":
                uiManager.PlusticketNumber(3000);
                break;
        }

        if (_icon != null)
        {
            _purchaseButton.gameObject.GetComponent<Image>().sprite = _icon;
            _purchaseButton.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            _purchaseButton.interactable = false;
        }
    }
    // private void RemoveAdsPack()
    // {
    //     ResourceManager.RemoveAds = true;
    //     // GameEventManager.PurchaseAds?.Invoke();
    // }
}