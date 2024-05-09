using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.PictureColoring
{
    public class HintButton : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private TextMeshProUGUI hintAmountText = null;
        [SerializeField] private GameObject      adsIcon;

        [SerializeField] private MMF_Player hintFeedback;

        private int _hintAmount;

        #endregion

        #region Unity Methods

        private void Start()
        {
            UpdateUI();

            CurrencyManager.Instance.OnCurrencyChanged += (string obj) => { UpdateUI(); };
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        #endregion

        #region Private Methods

        private void UpdateUI()
        {
            _hintAmount         = CurrencyManager.Instance.GetAmount("hints");
            hintAmountText.text = _hintAmount.ToString();
            if (_hintAmount > 0)
            {
                hintFeedback?.PlayFeedbacks();
                adsIcon.SetActive(false);
            }
            else
            {
                adsIcon.SetActive(true);
                hintFeedback?.StopFeedbacks();
            }
        }

        #endregion
    }
}