using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace BBG.PictureColoring
{
    public class VipPurchasePopup : Popup
    {
        [SerializeField] private GameObject _vipPurchasedSuccessPopup;
        [SerializeField] private GameObject _iapContainer;
        
        private LevelData _levelData;

        public override void OnShowing(object[] inData)
        {
            MaxMediationWrapper.Instance.bannerAdManager.HideBanner();
            base.OnShowing(inData);
            if (inData != null && inData.Length > 0)
            {
                _levelData = (LevelData)inData[0];
            }
            _vipPurchasedSuccessPopup.SetActive(false);
        }

        public override void OnHiding(bool cancelled)
        {
            base.OnHiding(cancelled);
            MaxMediationWrapper.Instance.bannerAdManager.ShowBanner();
        }

        public void OnUnlockClicked()
        {
            MaxMediationWrapper.Instance.ShowRewardAd(() =>
            {
                GameManager.Instance.UnlockLevel(_levelData);
                Hide(false);
            });
        }

        public void OnVipClicked()
        {
            PopupManager.Instance.Show("vip_purchase_popup");
            Hide(false);
        }

        #region Purchases

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
#if USE_MAX_SDK
            MaxMediationWrapper.Instance.bannerAdManager.HideBanner();
#endif
        }

        public void OnPurchasedMonthlySubscription()
        {
            PlayerPrefs.SetInt("MonthlySubscription", 1);
            PlayerPrefs.SetInt("MonthlySubscription_Start", System.DateTime.Now.DayOfYear);
            PlayerPrefs.Save();
#if USE_MAX_SDK
            MaxMediationWrapper.Instance.bannerAdManager.HideBanner();
#endif
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
#if USE_MAX_SDK
            MaxMediationWrapper.Instance.bannerAdManager.HideBanner();
#endif
        }

        public void OnPolicyClicked()
        {
            Application.OpenURL("https://sexy-coloring-book.blogspot.com/2023/10/sexy-coloring-book-for-adults-privacy.html");
        }

        public void OnTermServiceClicked()
        {
            Application.OpenURL("https://sexy-coloring-book.blogspot.com/2023/10/sexy-coloring-book-for-adults-term-of.html");
        }

        public void OnPurchasedFailed(Product product, PurchaseFailureDescription reason)
        {
            Debug.LogError($"Product: {product.definition.id} Purchase Failed: {reason.message}");
        }

        #endregion
    }
}