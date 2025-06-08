using System;
using System.Collections;
using System.Collections.Generic;
using CandyCoded.HapticFeedback;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour
{
    private string coin100 = "coin100";
    UiManager uiManager;

    private void Start()
    {
        uiManager = FindObjectOfType<UiManager>();
    }

    public void OnPurchaseCompleted(Product product)
    {
        if(product.definition.id == coin100)
        {
            // Assuming you have a method to add coins in UiManager
            uiManager.PlusticketNumber(100);
            Debug.Log("Purchase successful: " + product.definition.id);
        }
    }
    public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
    {
        Debug.LogError("Purchase failed: " + product.definition.id + ", Reason: " + reason);
    }
}
