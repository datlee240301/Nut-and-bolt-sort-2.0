using System;
using System.Collections;
using System.Collections.Generic;
using CandyCoded.HapticFeedback;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    private string Pack1 = "100coin.diamondsort.pack1";
    private string Pack2 = "300coin.diamondsort.pack2";
    private string Pack3 = "500coin.diamondsort.pack3";
    private string Pack4 = "900coin.diamondsort.pack4";
    private string Pack5 = "1200coin.diamondsort.pack5";
    private string Pack6 = "1500coin.diamondsort.pack6";
    private string Pack7 = "2000coin.diamondsort.pack7";
    private string Pack8 = "2500coin.diamondsort.pack8";
    private string Pack9 = "3000coin.diamondsort.pack9";
    [SerializeField] Button coin100Button;
    [SerializeField] Button coin300Button;
    [SerializeField] Button coin500Button;
    [SerializeField] Button coin900Button;
    [SerializeField] Button coin1200Button;
    [SerializeField] Button coin1500Button;
    [SerializeField] Button coin20000Button;
    [SerializeField] Button coin2500Button;
    [SerializeField] Button coin3000Button;

    UiManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
    }

    public void OnPurchaseCompleted(Product product)
    {
        if (product.definition.id == Pack1)
        {
            // Assuming you have a method to add coins in UiManager
            uiManager.PlusticketNumber(100);
            Debug.Log("Purchase successful: " + product.definition.id);
        }
        else if (product.definition.id == Pack2)
        {
            uiManager.PlusticketNumber(300);
        }
        else if (product.definition.id == Pack3)
        {
            uiManager.PlusticketNumber(500);
        }
        else if (product.definition.id == Pack4)
        {
            uiManager.PlusticketNumber(900);
        }
        else if (product.definition.id == Pack5)
        {
            uiManager.PlusticketNumber(1200);
        }
        else if (product.definition.id == Pack6)
        {
            uiManager.PlusticketNumber(1500);
        }
        else if (product.definition.id == Pack7)
        {
            uiManager.PlusticketNumber(2000);
        }
        else if (product.definition.id == Pack8)
        {
            uiManager.PlusticketNumber(2500);
        }
        else if (product.definition.id == Pack9)
        {
            uiManager.PlusticketNumber(3000);
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
    {
        Debug.LogError("Purchase failed: " + product.definition.id + ", Reason: " + reason);
    }

    public void OnProductFetched(Product product)
    {
        if (product.definition.id == Pack1)
        {
            UpdateButtonPrice(coin100Button, product);
        }
        else if (product.definition.id == Pack2)
        {
            UpdateButtonPrice(coin300Button, product);
        }
        else if (product.definition.id == Pack3)
        {
            UpdateButtonPrice(coin500Button, product);
        }
        else if (product.definition.id == Pack4)
        {
            UpdateButtonPrice(coin900Button, product);
        }
        else if (product.definition.id == Pack5)
        {
            UpdateButtonPrice(coin1200Button, product);
        }
        else if (product.definition.id == Pack6)
        {
            UpdateButtonPrice(coin1500Button, product);
        }
        else if (product.definition.id == Pack7)
        {
            UpdateButtonPrice(coin20000Button, product);
        }
        else if (product.definition.id == Pack8)
        {
            UpdateButtonPrice(coin2500Button, product);
        }
        else if (product.definition.id == Pack9)
        {
            UpdateButtonPrice(coin3000Button, product);
        }
    }

    private void UpdateButtonPrice(Button button, Product product)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.text = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
        }
    }
}