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
    public const string Pack1 = "100coin.diamondsort.pack1";
    public const string Pack2 = "300coin.diamondsort.pack2";
    public const string Pack3 = "500coin.diamondsort.pack3";
    public const string Pack4 = "900coin.diamondsort.pack4";
    public const string Pack5 = "1200coin.diamondsort.pack5";
    public const string Pack6 = "1500coin.diamondsort.pack6";
    public const string Pack7 = "2000coin.diamondsort.pack7";
    public const string Pack8 = "2500coin.diamondsort.pack8";
    public const string Pack9 = "3000coin.diamondsort.pack9";
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
    UiManager _uiManager;

    private void Start()
    {
        _uiManager = FindObjectOfType<UiManager>();
        RegisterPurchase();
        RegisterEventButton();
    }

    protected virtual void RegisterPurchase()
    {
        StartCoroutine(IAPManager.Instance.CreateHandleProduct(this));
    }

    public void Setup(Product product, string code, string price)
    {
        _model = product;
        if (_price != null)
        {
            _price.text = price;
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
            // case MyPurchaseID.RemoveAds:
            //     RemoveAdsPack();
            //     break;
            case MyPurchaseID.Pack1:
                _uiManager.BuyTicket(100);
                break;
            case MyPurchaseID.Pack2:
                _uiManager.BuyTicket(250);
                break;
            case MyPurchaseID.Pack3:
                _uiManager.BuyTicket(500);
                break;
            case MyPurchaseID.Pack4:
                _uiManager.BuyTicket(1000);
                break;
            case MyPurchaseID.Pack5:
                _uiManager.BuyTicket(1500);
                break;
            case MyPurchaseID.Pack6:
                _uiManager.BuyTicket(3000);
                break;
            case MyPurchaseID.Pack7:
                _uiManager.BuyTicket(4500);
                break;
            case MyPurchaseID.Pack8:
                _uiManager.BuyTicket(6000);
                break;
            case MyPurchaseID.Pack9:
                _uiManager.BuyTicket(9000);
                break;
        }

        if (_icon != null)
        {
            _purchaseButton.gameObject.GetComponent<Image>().sprite = _icon;
            _purchaseButton.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
            _purchaseButton.interactable = false;
        }
    }
    
    private void AddCoin(int amount)
    {
        
    }
    
    private void RemoveAdsPack()
    {
    }
}