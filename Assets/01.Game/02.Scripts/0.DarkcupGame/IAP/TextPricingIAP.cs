using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkcupGames;
using UnityEngine.Purchasing;

public class TextPricingIAP : MonoBehaviour
{
    private TextMeshProUGUI txt;
    public void OnProductFetched(Product product)
    {
        if (txt == null) txt = GetComponent<TextMeshProUGUI>();
        Debug.LogError("OnProductFetched " + product.definition.id);
        txt.text = product.metadata.localizedPriceString;
    }
}