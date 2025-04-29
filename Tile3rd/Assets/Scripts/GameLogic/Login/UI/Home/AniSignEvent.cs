using CSFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AniSignEvent : WindowUI
{
    protected override void on_create()
    {
        destroy_ui(this);
    }
}
