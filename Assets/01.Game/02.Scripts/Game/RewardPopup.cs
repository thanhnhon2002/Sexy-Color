using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BBG.PictureColoring
{
    public class RewardPopup : Popup
    {
        [SerializeField] private TextMeshProUGUI _countDownText;

        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            // DOVirtual.Int(5, 0, 5, (value) =>
            // {
            //     _countDownText.text = $"Video starts in {value}s";
            // }).onComplete += OnClaimAllButtonClicked;
        }

        public void OnClaimAllButtonClicked()
        {
            Hide(false);
            MaxMediationWrapper.Instance.ShowRewardAd(() =>
            {
                CurrencyManager.Instance.Give("hints", 3);
               
            });
        }

        public void OnClaimButtonClicked()
        {
            Hide(false);
            MaxMediationWrapper.Instance.ShowInterstitital(1, "reward_popup");
            CurrencyManager.Instance.Give("hints", 1);
         
        }
    }
}