using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Spine.Unity;

[System.Serializable]
public class RandomSpineAnimation
{
    public string AnimationName;
    public bool Loop;
}

[RequireComponent(typeof(SkeletonGraphic))]
public class RandomTimeSpineController : CSBehaviour
{
    public float MinGapTime = 6;
    public float MaxGapTime = 12;
    public List<string> RandomAnimationNameList;

    private SkeletonGraphic _spine;
    private float _next_play_time;
    private string _next_play_animation_name;
    private float _escape_time;

    private void Awake ()
    {
        _spine = GetComponent<SkeletonGraphic>();
        _spine.AnimationState.Complete += on_track_complete;

        _next_play_time = 0;
        _next_play_animation_name = null;
        _escape_time = 0;

        if (RandomAnimationNameList == null || RandomAnimationNameList.Count == 0)
            RandomAnimationNameList = new List<string> { _spine.startingAnimation };

        prepare_next_animation();
    }

    private void OnDestroy () 
    {
        _spine.AnimationState.Complete -= on_track_complete;
    }

    private void prepare_next_animation ()
    {
        _spine.enabled = false;

        _next_play_time = Mathf.Max(UnityEngine.Random.Range(MinGapTime, MaxGapTime), 0.001f);
        _next_play_animation_name = RandomAnimationNameList[UnityEngine.Random.Range(0, RandomAnimationNameList.Count)];
        _escape_time = 0;
    }

    private void on_track_complete (Spine.TrackEntry trackEntry)
    {
        prepare_next_animation();
    }

    private void Update ()
    {
        if (_next_play_time > 0)
        {
            _escape_time += Time.deltaTime;
            if (_escape_time >= _next_play_time)
            {
                _spine.enabled = true;
                _next_play_time = 0;

                _spine.AnimationState.ClearTracks();
                _spine.AnimationState.SetAnimation(0, _next_play_animation_name, false);
            }
        }
    }
}