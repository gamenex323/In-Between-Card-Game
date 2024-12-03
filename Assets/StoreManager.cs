using UnityEngine;
using UnityEngine.Purchasing;

public class StoreManager : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    // Replace "your_product_id" with the actual product ID
    [SerializeField]
    private string[] productID;
    private static int productIDNo;
    void Start()
    {
        InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        // Add your product ID to the builder
        for(int i = 0; i < productID.Length; i++)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(productID[i], ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }

    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Initialization failed: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("Initialization failed: " + error + ", " + message);
    }

    public void PurchaseProduct(int i)
    {
        productIDNo = i;
        if (storeController != null)
        {
            Product product = storeController.products.WithID(productID[i]);

            if (product != null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("Product not available for purchase.");
            }
        }
        else
        {
            Debug.Log("Store controller is not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Server.Instance.UpdateBalancePurchase(productIDNo);
        Debug.Log("Purchase successful: " + args.purchasedProduct.definition.id);
        // Add your logic for handling the successful purchase here

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log("Purchase failed: " + failureReason);
        // Add your logic for handling the failed purchase here
    }
}