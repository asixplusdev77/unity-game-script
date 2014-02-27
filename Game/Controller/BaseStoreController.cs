#define ANDROID_AMAZONN
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Events;

using UnityEngine;

public class GameStoreMessages {

    public static string purchaseStarted = "store-purchase-started";
    public static string purchaseSuccess = "store-purchase-success";
    public static string purchaseFailed = "store-purchase-failed";

    public static string purchaseThirdPartyStarted = "store-third-party-purchase-started";
    public static string purchaseThirdPartySuccess = "store-third-party-purchase-success";
    public static string purchaseThirdPartyFailed = "store-third-party-purchase-failed";

}

public class GameStorePurchaseRecord : DataObjectItem {
    public bool successful = false;
    public object data;
    public string receipt = "";
    public DateTime datePurchased;
    public string messageTitle = "";
    public string messageDescription = "";

    public GameStorePurchaseRecord() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        successful = false;
        data = null;
        receipt = "";
        datePurchased = DateTime.Now;
        messageTitle = "";
        messageDescription = "";
    }

    public static GameStorePurchaseRecord Create(
        bool success,
        object data,
        string receipt,
        string title,
        string message) {

        GameStorePurchaseRecord record = new GameStorePurchaseRecord();
        record.successful = success;
        record.data = data;
        record.receipt = receipt;
        record.datePurchased = DateTime.Now;
        record.messageTitle = title;
        record.messageDescription = message;

        return record;
    }
}

public class GameStorePurchaseDataItem {

    public GameStorePurchaseType gameStorePurchaseType = GameStorePurchaseType.LOCAL;
    public GameProduct product;
    public double quantity;
}

public enum GameStorePurchaseType {
    LOCAL,
    SERVER,
    EXTERNAL_GOOGLE_PLAY,
    EXTERNAL_APPLE_ITUNES,
    EXTERNAL_AMAZON,
    EXTERNAL_SAMSUNG
}

public class GameStorePurchaseData {

    public List<GameStorePurchaseDataItem> items;

    public GameStorePurchaseData() {
        Reset();
    }

    public void Reset() {
        items = new List<GameStorePurchaseDataItem>();
    }

    public void Add(GameStorePurchaseDataItem dataItem) {
        if(!items.Contains(dataItem)) {
            items.Add(dataItem);
        }
    }

    public static GameStorePurchaseData PurchaseData(string productCode, double quantity) {
        GameStorePurchaseData data = new GameStorePurchaseData();
        GameStorePurchaseDataItem dataItem = new GameStorePurchaseDataItem();
        dataItem.gameStorePurchaseType = GameStorePurchaseType.LOCAL;
        dataItem.product = GameProducts.Instance.GetById(productCode);
        dataItem.quantity = quantity;
        data.Add(dataItem);
        return data;
    }

}

public class BaseStoreController : MonoBehaviour {

    public virtual void Awake() {

    }

    public virtual void Start() {

    }

    public virtual void Init() {

    }

    public virtual void OnEnable() {

        Messenger<GameStorePurchaseData>.AddListener(GameStoreMessages.purchaseStarted, onStorePurchaseStarted);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseSuccess, onStorePurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseFailed, onStorePurchaseFailed);
        
        Messenger<GameStorePurchaseData>.AddListener(GameStoreMessages.purchaseStarted, onStoreThirdPartyPurchaseStarted);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseSuccess, onStoreThirdPartyPurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.AddListener(GameStoreMessages.purchaseFailed, onStoreThirdPartyPurchaseFailed);
    }
    
    public virtual void OnDisable() {
        
        Messenger<GameStorePurchaseData>.RemoveListener(GameStoreMessages.purchaseStarted, onStorePurchaseStarted);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseSuccess, onStorePurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseFailed, onStorePurchaseFailed);
        
        Messenger<GameStorePurchaseData>.RemoveListener(GameStoreMessages.purchaseStarted, onStoreThirdPartyPurchaseStarted);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseSuccess, onStoreThirdPartyPurchaseSuccess);
        Messenger<GameStorePurchaseRecord>.RemoveListener(GameStoreMessages.purchaseFailed, onStoreThirdPartyPurchaseFailed);
    }

    public virtual void onStorePurchaseStarted(GameStorePurchaseData data) {

    }

    public virtual void onStorePurchaseSuccess(GameStorePurchaseRecord data) {        
        UINotificationDisplay.Instance.QueueInfo(data.messageTitle, data.messageDescription);
    }

    public virtual void onStorePurchaseFailed(GameStorePurchaseRecord data) {        
        UINotificationDisplay.Instance.QueueError(data.messageTitle, data.messageDescription);
    }

    public virtual void onStoreThirdPartyPurchaseStarted(GameStorePurchaseData data) {

    }

    public virtual void onStoreThirdPartyPurchaseSuccess(GameStorePurchaseRecord data) {
        
        //GameStoreController.HandleCurrencyPurchase(item.product, item.quantity);   
    }

    public virtual void onStoreThirdPartyPurchaseFailed(GameStorePurchaseRecord data) {

    }

    public GameStorePurchaseDataItem itemPurchasing;

    public virtual void purchase(GameStorePurchaseData data) {

        foreach(GameStorePurchaseDataItem item in data.items) {

            if(item.product != null) {
    
                if(item.product.type == GameProductInfoType.currencyReal) {
                    // do third party process and event    
                    
                    itemPurchasing = item;

                    purchaseThirdParty(item.product, item.quantity);

                    //GameStoreController.HandleCurrencyPurchase(item.product, item.quantity);    
                }
                else {
                    // do local or server process and event

                    if(checkIfCanPurchase(item.product)) { // has the money
    
                        GameStoreController.HandlePurchase(item.product, item.quantity);
                    }
                    else {

                        GameStoreController.BroadcastPurchaseFailed(
                            GameStorePurchaseRecord.Create(false,
                                data, "",
                                "Purchase Unsuccessful",
                                "Not enough coins to purchase. Earn more coins by playing or training."));

                        //UINotificationDisplay.Instance.QueueError("Purchase Unsuccessful",
                         //   "Not enough coins to purchase. Earn more coins by playing or training.");
    
                    }
                }
            }

            // TODO handle multiple events, for now only purchase one at a time...
            break;
        }
    }

    public virtual void purchase(string productId, double quantity) {
       GameStoreController.Purchase(
            GameStorePurchaseData.PurchaseData(productId, quantity));
    }

    public virtual void broadcastPurchaseStarted(GameStorePurchaseData data) {
        Messenger<GameStorePurchaseData>.Broadcast(GameStoreMessages.purchaseStarted, data);
    }

    public virtual void broadcastPurchaseSuccess(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseSuccess, data);
    }

    public virtual void broadcastPurchaseFailed(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseFailed, data);
    }

    public virtual void broadcastThirdPartyPurchaseStarted(GameStorePurchaseData data) {
        Messenger<GameStorePurchaseData>.Broadcast(GameStoreMessages.purchaseThirdPartyStarted, data);
    }

    public virtual void broadcastThirdPartyPurchaseSuccess(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseThirdPartySuccess, data);
    }

    public virtual void broadcastThirdPartyPurchaseFailed(GameStorePurchaseRecord data) {
        Messenger<GameStorePurchaseRecord>.Broadcast(GameStoreMessages.purchaseThirdPartyFailed, data);
    }

    public virtual bool checkIfCanPurchase(GameProduct product) {
        double currentCurrency = GameProfileRPGs.Current.GetCurrency();

        double productCost = double.Parse(product.GetDefaultProductInfoByLocale().price);

        if(currentCurrency > productCost) {
            return true;
        }

        return false;
    }

    public virtual void purchaseLocal(GameProduct gameProduct, double quantity) {

        // server
        // event
    }

    public virtual void purchaseServer(GameProduct gameProduct, double quantity) {

    }

    public virtual void purchaseThirdParty(GameProduct gameProduct, double quantity) {
        ProductPurchase.PurchaseProduct(gameProduct.GetCurrentProductInfoByLocale().productId, (int)quantity);
    }

    public virtual void handlePurchase(GameProduct gameProduct, double quantity) {

        // HANDLE ACCOUNTING

        double currentCurrency = GameProfileRPGs.Current.GetCurrency();
        double productCost = double.Parse(gameProduct.GetDefaultProductInfoByLocale().price);

        // TODO quantity...

        if(currentCurrency > productCost) {
            // can buy

            GameProfileRPGs.Current.SubtractCurrency(productCost);

            // HANDLE INVENTORY
            GameStoreController.HandleInventory(gameProduct, quantity);
        }
    }

    public virtual void handleInventory(GameProduct gameProduct, double quantity) {

        string message = "Enjoy your new purchase.";

        if(gameProduct.type == GameProductType.rpgUpgrade) {
            // Add upgrades

            double val =  gameProduct.GetDefaultProductInfoByLocale().quantity;
            GameProfileRPGs.Current.AddUpgrades(val);

            message = "Advance your character with your upgrades and get to top of the game";

        }
        else if(gameProduct.type == GameProductType.powerup) {
            // Add upgrades

            if(gameProduct.code.Contains("rpg-recharge-full")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergyAndHealth(1f, 1f);
                message = "Recharging your health + energy...";
            }
            else if(gameProduct.code.Contains("rpg-recharge-health")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressHealth(1f);
                message = "Recharging your health...";
            }
            else if(gameProduct.code.Contains("rpg-recharge-energy")) {
                GameProfileCharacters.Current.CurrentCharacterAddGamePlayerProgressEnergy(1f);
                message = "Recharging your health...";
            }

        }
        else if(gameProduct.type == GameProductType.currency) {
            // Add skraight cash moneh
           GameStoreController.HandleCurrencyPurchase(gameProduct, quantity);

        }
        else if(gameProduct.type == GameProductType.characterSkin) {
            // TODO lookup skin and apply

            /*
            GameCharacterSkin skin = GameCharacterSkins.Instance.GetById(productCodeUse);
            GameCharacterSkinItemRPG rpg = skin.GetGameCharacterSkinByData(productCharacterUse, weaponType);
            if(rpg != null) {
                GameProfileCharacters.Current.SetCurrentCharacterCostumeCode(rpg.prefab);
            }
            */
        }

        GameStoreController.BroadcastPurchaseSuccess(
            GameStorePurchaseRecord.Create(true,
                gameProduct, "",
                "Purchase Successful",
                message));

    }

    public virtual void handleCurrencyPurchase(GameProduct gameProduct, double quantity) {
                
        Debug.Log("GameStoreController:handleCurrencyPurchase:productId:" + gameProduct.code);

        if(gameProduct.code == "currency-tier-1") {
            GameProfileRPGs.Current.AddCurrency(1000);
        }
        else if(gameProduct.code == "currency-tier-2") {
            GameProfileRPGs.Current.AddCurrency(3500);
        }
        else if(gameProduct.code == "currency-tier-3") {
            GameProfileRPGs.Current.AddCurrency(15000);
        }
        else if(gameProduct.code == "currency-tier-5") {
            GameProfileRPGs.Current.AddCurrency(50000);
        }
        else if(gameProduct.code == "currency-tier-10") {
            GameProfileRPGs.Current.AddCurrency(100000);
        }
        else if(gameProduct.code == "currency-tier-20") {
            GameProfileRPGs.Current.AddCurrency(250000);
        }
        else if(gameProduct.code == "currency-tier-50") {
            GameProfileRPGs.Current.AddCurrency(1000000);
        }
    }

    // THIRD PARTY EVENTS

    public bool purchaseProcessCompleted = false;

    public void HandleCancel() {

    }

    public void HandleSuccess() {
        GameStoreController.HandleCurrencyPurchase(itemPurchasing.product, itemPurchasing.quantity);  

    }

    public void HandleError() {

    }

    public void SetContentAccessPermissions(string code) {

    }

#if UNITY_IPHONE
    void purchaseFailed(string error) {
        purchaseProcessCompleted = true;
        HandleError();
        LogUtil.LogProduct( "purchase failed with error: " + error );
        itemPurchasing = null;
    }

    void purchaseCancelled(string error) {
        purchaseProcessCompleted = true;
        HandleCancel();
        LogUtil.LogProduct( "purchase cancelled with error: " + error );
        itemPurchasing = null;
    }

    void purchaseSuccessful(StoreKitTransaction transaction) {
        LogUtil.LogProduct( "SCREEN purchased product: " + transaction.productIdentifier + ", quantity: " + transaction.quantity );
        //transaction.base64EncodedTransactionReceipt
        SetContentAccessPermissions(transaction.productIdentifier);
        Contents.SetContentAccessTransaction(transaction.productIdentifier, transaction.productIdentifier,
                transaction.base64EncodedTransactionReceipt, transaction.quantity, true);
        purchaseProcessCompleted = true;

        GameStorePurchaseRecord record = 
            GameStorePurchaseRecord.Create(
                true, transaction.ToJson(), 
                transaction.base64EncodedTransactionReceipt, 
                "Purchase Complete:" + itemPurchasing.product.display_name, itemPurchasing.product.description);

        itemPurchasing = null;

        broadcastThirdPartyPurchaseSuccess(record);
    }

#endif
        
                
#if UNITY_ANDROID
#if ANDROID_AMAZON

    void itemDataRequestFailedEvent() {
        LogUtil.LogProduct( "ANDROID_AMAZON itemDataRequestFailedEvent:");
    }
    
    void itemDataRequestFinishedEvent(List<string> unavailableSkus, List<AmazonItem> products) {
        LogUtil.LogProduct( "ANDROID_AMAZON itemDataRequestFinishedEvent:");
        
        foreach(string sku in unavailableSkus) {                        
            LogUtil.LogProduct( "ANDROID_AMAZON AmazonItem: unavailableSkus:" + sku);
        }
        
        LogUtil.LogProduct( "ANDROID_AMAZON itemDataRequestFinishedEvent: products.Count:" + products.Count);
        foreach(AmazonItem item in products) {                  
            LogUtil.LogProduct( "ANDROID_AMAZON AmazonItem: sku:" + item.sku + "\r\n title:" + item.title + "\r\n description:" + item.description + "\r\n price:" + item.price);
        }
    }
    
    void onGetUserIdResponseEvent(string userId) {
        LogUtil.LogProduct( "ANDROID_AMAZON onGetUserIdResponseEvent: userId:" + userId);
    }
    
    void onSdkAvailableEvent(bool debug) {
        LogUtil.LogProduct( "ANDROID_AMAZON onSdkAvailableEvent: debug:" + debug);
    }
    
    void purchaseFailedEvent() {
        LogUtil.LogProduct( "ANDROID_AMAZON purchaseFailedEvent");
        HandleError();
    }
    
    void purchaseSuccessfulEvent(AmazonReceipt transaction) {
        LogUtil.LogProduct( "ANDROID_AMAZON purchaseSuccessfulEvent: transaction.sku:" + transaction.sku + "\r\n token:" + transaction.token + "\r\n type:" + transaction.type);
        SetContentAccessPermissions(transaction.sku);
        Contents.Instance.SetContentAccessTransaction(transaction.sku, transaction.sku, 
                transaction.token, 1, true);
        purchaseProcessCompleted = true;
        HandleSuccess();
    }
    
    void purchaseUpdatesRequestFailedEvent() {
            LogUtil.LogProduct( "ANDROID_AMAZON purchaseUpdatesRequestFailedEvent:");
    }
    
    void purchaseUpdatesRequestSuccessfulEvent(List<string> unavailableSkus, List<AmazonReceipt> transactions) {
            LogUtil.LogProduct( "ANDROID_AMAZON purchaseUpdatesRequestSuccessfulEvent:");
            foreach(string sku in unavailableSkus) {                        
                    LogUtil.LogProduct( "ANDROID_AMAZON AmazonReceipt: unavailableSkus:" + sku);
            }
            
            LogUtil.LogProduct( "ANDROID_AMAZON purchaseUpdatesRequestSuccessfulEvent: transactions.Count:" + transactions.Count);
            foreach(AmazonReceipt transaction in transactions) {                    
                    LogUtil.LogProduct( "ANDROID_AMAZON AmazonReceipt: sku:" + transaction.sku  + "\r\n token:" + transaction.token + "\r\n type:" + transaction.type);
                    
                    string productId = transaction.sku.Replace(GamePacks.currentGameBundle + ".", "");
                    Contents.Instance.SetGlobalContentAccess(productId);
                    Contents.Instance.SetContentAccessTransaction(productId, transaction.sku,
                            transaction.token, 1, true);
            }
    }
#else
    void billingSupportedEvent(bool success) {
        LogUtil.LogProduct( "billingSupportedEvent: " + success );
        //IABAndroid.restoreTransactions();
    }
    
    void purchaseSucceededEvent(string productId) {
        LogUtil.LogProduct( "purchaseSucceededEvent product: " + productId );
        SetContentAccessPermissions(productId);
        purchaseProcessCompleted = true;
        HandleSuccess();
    }       
    
    void purchaseCancelledEvent(string productId) {
        LogUtil.LogProduct( "purchaseCancelledEvent product: " + productId );
    }
            
    void purchaseRefundedEvent(string productId) {
        LogUtil.LogProduct( "purchaseRefundedEvent product: " + productId );
    }
            
    void purchaseSignatureVerifiedEvent(string signedData, string signature) {
        LogUtil.LogProduct( "purchaseSignatureVerifiedEvent signedData: " + signedData + " signature:" + signature);
    }
    
    void purchaseFailedEvent(string productId) {
        LogUtil.LogProduct( "purchaseFailedEvent product: " + productId );
        HandleError();
    }
    
    void transactionsRestoredEvent() {
        LogUtil.LogProduct( "transactionsRestored");
    }
    
    void transactionRestoreFailedEvent(string error) {
        LogUtil.LogProduct( "transactionRestoreFailedEvent product: " + error );
    }
#endif
#endif
        
}