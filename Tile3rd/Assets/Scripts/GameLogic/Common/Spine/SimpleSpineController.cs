using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Spine.Unity;

[System.Serializable]
public class SimpleSpineAnimation
{
    public string AnimationName;
    public bool Loop;
    public float Delay = 0.0f;
}

[RequireComponent(typeof(SkeletonGraphic))]
public class SimpleSpineController : CSBehaviour
{
    public List<SimpleSpineAnimation> AnimationSequence;

    private SkeletonGraphic _spine;

    private void Awake ()
    {
        _spine = GetComponent<SkeletonGraphic>();
    }
    
    private void OnEnable() 
    {
        if (!string.IsNullOrEmpty(_spine.startingAnimation))
        {
            _spine.AnimationState.ClearTracks();
            // _spine.AnimationState.ClearTrack(0);
            _spine.AnimationState.SetAnimation(0, _spine.startingAnimation, _spine.startingLoop);
            if (AnimationSequence != null && AnimationSequence.Count > 0)
            {
                foreach (var animation in AnimationSequence)
                    _spine.AnimationState.AddAnimation(0, animation.AnimationName, animation.Loop, animation.Delay);
            }
            _spine.Update(0);
        }
    }
}