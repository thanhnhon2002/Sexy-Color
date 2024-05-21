using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using UnityEngine.Purchasing.Security;
using Unity.Services.Core;
using System.Collections.Generic;
using BBG;
using MoreMountains.Tools;
using UnityEngine.Purchasing.Extension;

public class MyIAPManager : MMPersistentSingleton<MyIAPManager>//, IStoreListener
{

    public void OnPurchaseComplete(Product product)
    {
        switch (product.definition.id)
        {
            case "color.iap.50hints":
                OnPurchased50HintsCompleted();
                break;
            case "color.weekly.vip":    
                OnPurchasedWeeklySubscription();
                break;
            case "color.monthly.vip":
                OnPurchasedMonthlySubscription();
                break;
            case "color.unlimited.hints":
                OnPurchasedUnlimitedHints();
                break;
            case "color.iap.removeads":
                OnPurchasedRemoveAds();
                break;
        }
    }
    
    public void OnPurchased50HintsCompleted()
    {
        CurrencyManager.Instance.Give("hints", 50);
    }

    public void OnPurchasedCompleted(Product product)
    {
        Debug.Log($"Product: {product.definition.id} Purchase Completed");
    }

    public void OnPurchasedWeeklySubscription()
    {
        PlayerPrefs.SetInt("WeeklySubscription", 1);
        PlayerPrefs.SetInt("WeeklySubscription_Start", System.DateTime.Now.DayOfYear);
        PlayerPrefs.Save();
    }

    public void OnPurchasedMonthlySubscription()
    {
        PlayerPrefs.SetInt("MonthlySubscription", 1);
        PlayerPrefs.SetInt("MonthlySubscription_Start", System.DateTime.Now.DayOfYear);
        PlayerPrefs.Save();
    }

    public void OnPurchasedUnlimitedHints()
    {
        PlayerPrefs.SetInt("UnlimitedHints", 1);
        PlayerPrefs.Save();
    }

    public void OnPurchasedRemoveAds()
    {
        PlayerPrefs.SetInt("RemoveAds", 1);
        PlayerPrefs.Save();
    }

    public void OnPolicyClicked()
    {
        Application.OpenURL("https://sites.google.com/view/sexy-coloring-book-policy/home");
    }

    public void OnTermServiceClicked()
    {
        Application.OpenURL("https://sites.google.com/view/sexy-coloring-book-tos/home");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
    {
        Debug.LogError($"Product: {product.definition.id} Purchase Failed: {reason.message}");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
 
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
       
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        throw new NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError($"Product: {product.definition.id} Purchase Failed: {failureReason}");
    }
}