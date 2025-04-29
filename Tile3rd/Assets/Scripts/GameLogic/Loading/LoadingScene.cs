using CSFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScene : BaseLoadingScene
{
    private SchedulerFloat _fake_scheduler;

    protected override IEnumerator on_init (params object[] param_list)
    {
        set_progress(0.0f);
        _fake_scheduler = new SchedulerFloat();
        _fake_scheduler.Init(0.4f);
        return null;
    }

    private void set_progress (float progress)
    {
        LoadingProgress = progress;
    }

    protected override IEnumerator on_loading_process ()
    {
        while (!_fake_scheduler.IsArrived())
        {
            set_progress(_fake_scheduler.Percent());
            yield return new WaitForEndOfFrame();
        }

        set_progress(1.0f);
    }

    protected override IEnumerator on_cleanup()
    {
        yield return null;
    }

    protected override void on_tick(float dt)
    {
        _fake_scheduler.Tick(dt, false);
    }
}