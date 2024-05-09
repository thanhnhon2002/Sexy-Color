﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.Advertisements;

public class FireBaseManager : MonoBehaviour
{
    public static FireBaseManager Instance;

    public static bool firstTime   = true;
    private       int  rewardIndex = 0;
   

    bool                                 UIEnabled             = true;
    private string                       logText               = "";
    const   int                          kMaxLogSize           = 16382;
    Firebase.DependencyStatus            dependencyStatus      = Firebase.DependencyStatus.UnavailableOther;
    protected bool                       isFirebaseInitialized = false;
    public    Dictionary<string, object> Defaults              = new();
    public    Dictionary<string, object> FetchedData           = new();

    // Set to True when fetching process if finished
    public bool IsReady;
    
    private void Awake()
    {
        FireBaseManager[] check = FindObjectsOfType<FireBaseManager>();
        foreach (FireBaseManager searched in check)
        {
            if (searched != this)
            {
                Destroy(searched.gameObject);
            }
        }
        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                    "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        
        if (Instance == null)
            Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Initialize remote config, and set the default values.
    void InitializeFirebase()
    {
        // These are the values that are used if we haven't fetched data from the
        // server
        // yet, or if we ask for values that the server doesn't have:
        Defaults.Add("inter_ad_capping_time", 30);
        Defaults.Add("open_ad_capping_time", true);
        Defaults.Add("open_ad_on_off", true);
        Defaults.Add("level_show_rate", 2);
        Defaults.Add("offline_play_on_off", false);

        // Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(Defaults)
        //     .ContinueWithOnMainThread(task =>
        //     {
        //         // [END set_defaults]
        //         Debug.Log("RemoteConfig configured and ready!");
        //         isFirebaseInitialized = true;
        //         FetchDataAsync();
        //     });

        //Fire event App Open
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);
    }

    // [START fetch_async]
    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.
    // public Task FetchDataAsync()
    // {
    //     Debug.Log("Fetching data...");
    //     System.Threading.Tasks.Task fetchTask =
    //         Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
    //             TimeSpan.Zero);
    //     return fetchTask.ContinueWithOnMainThread(FetchComplete);
    // }
    //[END fetch_async]

    void FetchComplete(Task fetchTask)
    {
        // if (fetchTask.IsCanceled)
        // {
        //     Debug.Log("Fetch canceled.");
        // }
        // else if (fetchTask.IsFaulted)
        // {
        //     Debug.Log("Fetch encountered an error.");
        // }
        // else if (fetchTask.IsCompleted)
        // {
        //     Debug.Log("Fetch completed successfully!");
        // }
        //
        // var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        // switch (info.LastFetchStatus)
        // {
        //     case Firebase.RemoteConfig.LastFetchStatus.Success:
        //         Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
        //             .ContinueWithOnMainThread(task =>
        //             {
        //                 Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
        //                     info.FetchTime));
        //                 // DisplayData();
        //                 CacheData();
        //
        //                 IsReady = true;
        //             });
        //
        //         break;
        //     case Firebase.RemoteConfig.LastFetchStatus.Failure:
        //         switch (info.LastFetchFailureReason)
        //         {
        //             case Firebase.RemoteConfig.FetchFailureReason.Error:
        //                 Debug.Log("Fetch failed for unknown reason");
        //                 break;
        //             case Firebase.RemoteConfig.FetchFailureReason.Throttled:
        //                 Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
        //                 break;
        //         }
        //
        //         break;
        //     case Firebase.RemoteConfig.LastFetchStatus.Pending:
        //         Debug.Log("Latest Fetch call still pending.");
        //         break;
        // }
    }

    private void CacheData()
    {
        // FetchedData.Clear();
        // FetchedData.Add("inter_ad_capping_time",Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //     .GetValue("inter_ad_capping_time").LongValue);
        // FetchedData.Add("open_ad_capping_time",Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //     .GetValue("open_ad_capping_time").LongValue);
        // FetchedData.Add("open_ad_on_off",Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //     .GetValue("open_ad_on_off").BooleanValue);
        // FetchedData.Add("level_show_rate",Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //     .GetValue("level_show_rate").LongValue);
        // FetchedData.Add("offline_play_on_off",Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //     .GetValue("offline_play_on_off").BooleanValue);
    }

    // Display the currently loaded data.  If fetch has been called, this will be
    // the data fetched from the server.  Otherwise, it will be the defaults.
    // Note:  Firebase will cache this between sessions, so even if you haven't
    // called fetch yet, if it was called on a previous run of the program, you
    //  will still have data from the last time it was run.
    public void DisplayData()
    {
        // Debug.Log("Current Data:");
        // Debug.Log("min_capping_inter: " +
        //           Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //               .GetValue("min_capping_inter").LongValue);
        // //MaxMediationWrapper.Instance.LoadInterAdsCappingTime(Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        // //    .GetValue("min_capping_inter").LongValue);
        // Debug.Log("booster_mode: " +
        //           Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
        //               .GetValue("booster_mode").BooleanValue);
    }

    /// <summary>Get a fetched value. It'll wait until the fetching process is finished</summary>
    public async Task<object> GetValue(string key)
    {
        while (!IsReady)
        {
            await Task.Delay(100);
        }

        if (FetchedData.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }

    public void DisplayAllKeys()
    {
        // Debug.Log("Current Keys:");
        // System.Collections.Generic.IEnumerable<string> keys =
        //     Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Keys;
        // foreach (string key in keys)
        // {
        //     Debug.Log("    " + key);
        // }
        //
        // Debug.Log("GetKeysByPrefix(\"config_test_s\"):");
        // keys = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetKeysByPrefix("config_test_s");
        // foreach (string key in keys)
        // {
        //     Debug.Log("    " + key);
        // }
    }
}