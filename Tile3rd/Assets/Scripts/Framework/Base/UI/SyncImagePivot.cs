using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class SyncImagePivot : MonoBehaviour
{
    private void Awake ()
    {
        var rt = GetComponent<RectTransform>();
        if (rt)
        {
            var image = GetComponent<Image>();
            if (image && image.sprite)
            {
                var size = rt.rect.size;
                var pivot = image.sprite.pivot / image.sprite.rect.size;
                var delta_pivot = pivot - rt.pivot;
                var delta_position = new Vector3(delta_pivot.x * size.x, delta_pivot.y * size.y);
                rt.pivot = pivot;
                rt.localPosition += delta_position;
            }
        }
    }
}
