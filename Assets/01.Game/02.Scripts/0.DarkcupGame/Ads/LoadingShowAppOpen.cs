using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingShowAppOpen : MonoBehaviour
{
    [SerializeField] private bool showDebug;

    public float LOADING_TIME = 7f;
    //public PopupLoading popupLoading;
    public AdmobAppOpen appOpen;
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        //StartLoadingAndShowAppOpen(null);
    }
    public void StartLoadingAndShowAppOpen(System.Action onLoadFinished)
    {
        //popupLoading.gameObject.SetActive(true);
       // popupLoading.ShowLoading(LOADING_TIME, () =>{
            if (showDebug) Debug.Log("try showing app open");
            if (appOpen.IsAdsAvailable())
            {
                if (showDebug) Debug.Log("app open available, showing ads");
                appOpen.ShowAds(() =>
                {
                    if (showDebug) Debug.Log("this is app open closed");
                    //popupLoading.Close();
                    onLoadFinished?.Invoke();
                    //FirebaseManager.analytics.LogAdsAppOpenRecorded("admob", "loading");
                });
            }
            else
            {
                if (showDebug) Debug.Log("app open available failed");
                //popupLoading.Close();
                onLoadFinished?.Invoke();
            }
        
    }
}