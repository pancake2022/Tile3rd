using CSFramework;
using UnityEngine;

public class FollowUI : BaseUI
{
    protected Transform _target;
    protected Camera _target_camera;
    protected Camera _self_camera;
    protected Vector3 _follow_diff = Vector3.zero;

    public void FollowSceneTransform (Transform target)
    {
        var context = _ui_manager.Framework.Context;
        init(target, context.MainCamera, context.UICamera);
    }

    protected void init (Transform target, Camera target_camera, Camera self_camera)
    {
        _target = target;
        _target_camera = target_camera;
        _self_camera = self_camera;
        
        refresh();
    }

    protected void refresh ()
    {
        if (_target)
        {
            // var position = _target_camera.WorldToScreenPoint(_target.transform.position);
            // position = _self_camera.ScreenToWorldPoint(position);
            // position.z = 0;
            // todo local position
            transform.position = Utils.ConvertPosition(_target.transform.position, _target_camera, _self_camera);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0) + _follow_diff;
        }
    }

    private void LateUpdate() 
    {
        refresh();
    }
}