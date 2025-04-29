using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPath : CSFramework.CSBehaviour
{
    public List<Transform> PathPointList = null;
    private void Awake () 
    {
        PathPointList = new List<Transform>();
        for (var i = 0; i < this.transform.childCount; ++i)
        {
            PathPointList.Add(this.transform.GetChild(i));
        }
    }
}
