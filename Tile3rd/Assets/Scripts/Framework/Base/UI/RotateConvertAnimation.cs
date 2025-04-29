using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RotateConvertAnimation : MonoBehaviour
{
    public Vector3 RotateDirection;
    public Quaternion RotateQuaternionDirection;
    [Range(0.0f, 1.0f)]
    public float RotateProgress;

    private float _progress_dirty;

    private void Update()
    {
        if (_progress_dirty != RotateProgress)
        {
            _progress_dirty = RotateProgress;
            convert_rotation();
        }
    }

    private void convert_rotation ()
    {
        // transform.localEulerAngles = Vector3.Lerp(Vector3.zero, RotateDirection, RotateProgress);
        transform.localRotation = Quaternion.Lerp(Quaternion.Euler(Vector3.zero), RotateQuaternionDirection, RotateProgress);
    }
}
