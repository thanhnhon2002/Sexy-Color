using System;
using System.Collections;
using System.Collections.Generic;
using BBG;
using BBG.PictureColoring;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Event = Spine.Event;

public class HalfwayPopup : Popup
{
    [SerializeField] private SkeletonGraphic   boxAnim;
    [SerializeField] private Image             reward;
    [SerializeField] private Button            claimButton;
    

    private LevelData _levelData;
    private void Awake()
    {
        claimButton.onClick.AddListener(OnClaimButtonClick);
    }

    private void OnClaimButtonClick()
    {
        CurrencyManager.Instance.Give("hints", _levelData.hintsToAward);
        Hide(false);
    }

    public override void OnShowing(object[] inData)
    {
        base.OnShowing(inData);
        _levelData = inData[0] as LevelData;
        TrackEntry trackEntry = boxAnim.AnimationState.SetAnimation(0, "Unbox", false);
        trackEntry.Complete += OnAnimationEnd;
    }

    public override void OnHiding(bool cancelled)
    {
        base.OnHiding(cancelled);
        boxAnim.AnimationState.SetAnimation(0, "Shake", true);
        boxAnim.gameObject.SetActive(true);
        reward.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);
    }

    private void OnAnimationEnd(TrackEntry trackentry)
    {
        StartCoroutine(ShowRewardCo());
    }
    
    private IEnumerator ShowRewardCo()
    {
        yield return new WaitForSeconds(0.5f);
        boxAnim.gameObject.SetActive(false);
        PopupManager.Instance.Show("reward_popup",new object[]{}, (cancelled, data) =>
        {
            Hide(false);
        });
        yield return null;
    }
}
