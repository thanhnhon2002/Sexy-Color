// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Collections;
using System.Collections.Generic;

using GoogleMobileAds.Api;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
/// <summary>
/// Ads controller class to request and display reward based video ads.
/// </summary>
public class AdsController : MonoBehaviour
{

    [SerializeField] UnityEvent<NativeAd> OnAdLoaded;

    /// A test ad unit for custom native ads.

     [SerializeField] public string AdUnitId = "ca-app-pub-3082034395446486/2515277924";

    private bool nativeAdLoaded;
    private NativeAd nativeAd;
    public RawImage AdIconTexture;
    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(HandleInitCompleteAction);
    }

    /// <summary>
    /// Called by MobileAds when initialization is completed.
    /// </summary>
    private void HandleInitCompleteAction(InitializationStatus initstatus)
    {
        Debug.Log("NATIVE: Initialization complete ");
        foreach (var item in initstatus.getAdapterStatusMap())
        {
            Debug.Log(item.Key + " ---- " + item.Value);
        }
        nativeAdLoaded = false;
        RequestNativeAd();
    }
    /// <summary>
    /// Requests a CustomNativeAd.
    /// </summary>
    private void RequestNativeAd()
    {

        AdLoader adLoader = new AdLoader.Builder(AdUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += HandleCustomNativeAdLoaded;
        adLoader.OnAdFailedToLoad += HandleNativeAdFailedToLoad;

        adLoader.LoadAd(new AdRequest());
    }
    private void HandleCustomNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        Debug.Log("Admobs Loaded Successfully!");
        nativeAd = args.nativeAd;
        nativeAdLoaded = true;              
        OnAdLoaded?.Invoke(nativeAd);
      
    }

    /// <summary>
    /// Handles the native ad failing to load.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="args">Error information.</param>
    private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        string message = args.LoadAdError.GetMessage();
        MonoBehaviour.print("Ad Loader fail event received with message: " + message);
        Debug.LogError("Admobs Loaded Failed!" + message);
    }
}