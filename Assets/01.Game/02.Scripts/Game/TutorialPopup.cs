using System;
using System.Collections;
using System.Collections.Generic;
using BBG;
using Lean.Touch;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : Popup
{
    [SerializeField] private Animator animator;

    private int _currentIndex;
    public override void OnShowing(object[] inData)
    {
        animator.Play("Default");
        _currentIndex           =  0;
        LeanTouch.OnFingerSwipe += OnFingerSwipe;
    }

    private void OnFingerSwipe(LeanFinger obj)
    {
        if (obj.StartScreenPosition.x > obj.ScreenPosition.x)
        {
            PlayNextPage();
        }
    }

    private void OnDestroy()
    {
        LeanTouch.OnFingerSwipe -= OnFingerSwipe;
    }

    public void PlayNextPage()
    {
        _currentIndex++;
        if (_currentIndex > 2)
        {
            Hide(true);
            return;
        }
        animator.Play("Step" + _currentIndex);
    }
    
}
