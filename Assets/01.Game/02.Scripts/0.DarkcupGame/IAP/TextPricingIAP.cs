using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkcupGames;
using UnityEngine.Purchasing;

public class TextPricingIAP : MonoBehaviour
{
    public TextMeshProUGUI txt;
    public string id;

    private void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        //if (ShopIAPManager.Instance == null) return;
        //if (ShopIAPManager.Instance.IsInitDone() == false) return;
        id = transform.GetComponentInParent<CodelessIAPButton>().productId;
        Dictionary<string, string> prices = MyIAPManager.Instance.prices;
        if (prices.ContainsKey(id) == false) return;
        txt.text = prices[id];
    }
}