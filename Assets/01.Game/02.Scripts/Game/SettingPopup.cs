using System;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class SettingPopup : Popup
    {
        [SerializeField] private GameObject confirmClearCachePopup;
        [SerializeField] private MMSwitch   _vibrationSwitch;
        [SerializeField] private Button _cancelSubscriptionButton;
        

        private void Awake()
        {
            _vibrationSwitch.SwitchOn.AddListener(OnVibrationOn);
            _vibrationSwitch.SwitchOff.AddListener(OnVibrationOff);
        }

        private void OnVibrationOff()
        {
            PlayerPrefs.SetInt("Vibration_Setting", 0);
            PlayerPrefs.Save();
        }

        private void OnVibrationOn()
        {
            PlayerPrefs.SetInt("Vibration_Setting", 1);
            PlayerPrefs.Save();
        }

        public void OnPremiumButtonClicked()
        {
            PopupManager.Instance.Show("vip_purchase_popup");
        }

        public void OnChooseMaterialClicked(int index)
        {
            PlayerPrefs.SetInt("ColorMaterial", index);
            PlayerPrefs.Save();
        }

        public void OnClearCacheButtonClicked()
        {
            confirmClearCachePopup.SetActive(true);
        }

        public void OnCancelSubscriptionClicked()
        {
            // SaveManager.Instance.DeleteSaveData();
            Application.OpenURL("https://play.google.com/store/account/subscriptions");
        }

        public void OnPolicyClicked()
        {
            Application.OpenURL("https://sexy-coloring-book.blogspot.com/2023/10/sexy-coloring-book-for-adults-privacy.html");
        }
        
        public void OnTermServiceClicked()
        {
            Application.OpenURL("https://sexy-coloring-book.blogspot.com/2023/10/sexy-coloring-book-for-adults-term-of.html");
        }

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            if(PlayerPrefs.GetInt("MonthlySubscription", 0) == 1 || PlayerPrefs.GetInt("WeeklySubscription", 0) == 1)
            {
                _cancelSubscriptionButton.gameObject.SetActive(true);
            }
            else
            {
                _cancelSubscriptionButton.gameObject.SetActive(false);
            }
        }
    }
}