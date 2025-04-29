using CSFramework;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class M3GameHighLightCellUI : M3GameCellUI
{
    public static readonly Vector3 HighLightScale = new Vector3(1.12f, 1.12f, 1.12f);//点击缩放
    private SchedulerFloat _scale_scheduler = new SchedulerFloat().Init(0.2f);//缩放刷新速度

    public M3GameHighLightCellUI Init (M3Cell cell)//初始化
    {
        RefreshCell(cell);//刷新cell
        return this;
    }

    public M3GameHighLightCellUI SetHighLight (bool high_light, M3Cell cell)//设置可点击
    {
        if (high_light)
        {
            gameObject.SetActive(true);
            if (Cell != cell)
            {
                Init(cell).RefreshPosition();//初始化坐标
                _scale_scheduler.Reset();//初始化缩放
                transform.localScale = Vector3.one;//初始化大小
            }
        }
        else
        {
            Cell = null;
            gameObject.SetActive(false);
            transform.localScale = Vector3.one;
        }
        return this;
    }

    private void Update ()//运行进程中的点击缩放判断
    {
        if (_scale_scheduler.Tick(Time.deltaTime, false))
        {
            transform.localScale = HighLightScale;
        }
        else if (!_scale_scheduler.IsArrived())
        {
            transform.localScale = Vector3.Lerp(Vector3.one, HighLightScale, _scale_scheduler.Percent());
        }
    }
    public void HighLightTest()
    {
        Debug.Log("highlight");
    }
}