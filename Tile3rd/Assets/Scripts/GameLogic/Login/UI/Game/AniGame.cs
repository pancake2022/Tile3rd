using CSFramework;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AniGame : WindowUI
{
    public LevelwinUI LevelWin;

    public AniGame Init(LevelwinUI levelwin)//PanelUI的初始化
    {
        LevelWin = levelwin;
        return this;
    }

    public void LevelWinCountStart()
    {
        LevelWin.textCountStart = true;
    }
}
