using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private IStoreController storeController;
    private IExtensionProvider storeExtensionProvider;

    [System.Serializable]
    public class IAPProduct
    {
        public string productId;
        public int coinAmount;
    }

    [Header("Product Configs (match with IAP Catalog)")]
    public List<IAPProduct> products = new List<IAPProduct>();

    [Header("Buttons (match order of products)")]
    public List<Button> buttons = new List<Button>();

    private UiManager uiManager;

    void Start()
    {
        uiManager = FindObjectOfType<UiManager>();

#if FORCE_FAKE_IAP
        var module = StandardPurchasingModule.Instance(AppStore.fake);
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#else
        var module = StandardPurchasingModule.Instance(AppStore.GooglePlay);
#endif

        var builder = ConfigurationBuilder.Instance(module);

        foreach (var item in products)
        {
            builder.AddProduct(item.productId, ProductType.Consumable);
        }

        UnityPurchasing.Initialize(this, builder);

        for (int i = 0; i < buttons.Count && i < products.Count; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => BuyProduct(products[index].productId));
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensionProvider = extensions;

        UpdateAllButtonPrices();
        Debug.Log("IAP Initialized");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP Init Failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("IAP Init Failed: " + error + " | " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        foreach (var item in products)
        {
            if (args.purchasedProduct.definition.id == item.productId)
            {
                uiManager.PlusticketNumber(item.coinAmount);
                Debug.Log("Purchase success: " + item.productId);
                break;
            }
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"Purchase failed: {product.definition.id}, Reason: {reason}");
    }

    public void BuyProduct(string productId)
    {
        Debug.Log("Buying product: " + productId);
        if (storeController == null)
        {
            Debug.LogWarning("Store not initialized yet.");
            return;
        }

        Product product = storeController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogWarning("Product not available or not found: " + productId);
        }
    }

    private void UpdateAllButtonPrices()
    {
        for (int i = 0; i < buttons.Count && i < products.Count; i++)
        {
            UpdateButtonPrice(buttons[i], products[i].productId);
        }
    }

    private void UpdateButtonPrice(Button button, string productId)
    {
        if (storeController == null) return;

        Product product = storeController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = product.metadata.localizedPriceString + " $";
                text.fontSize = 40.8f;
            }
        }
    }
}
