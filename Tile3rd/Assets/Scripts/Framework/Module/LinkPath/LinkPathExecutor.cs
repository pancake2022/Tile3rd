using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSFramework;

public class LinkPathExecutor : CSBehaviour
{
    public delegate float SpeedValueDelegate();

    public LinkPath Path;
    private LinkPath _current_execute_path = null;
    private int _next_path_point_index = 0;
    private bool _is_pause = false;
    private bool _is_arrived = false;
    private System.Action _on_arrived = null;
    private SpeedValueDelegate _speed_value_delegate = null;

    public void Tick (float dt)
    {
        if (_current_execute_path && !_is_pause && !_is_arrived)
            update_execute(dt);
    }

    public void Pause ()
    {
        _is_pause = true;
    }

    public void Resume ()
    {
        _is_pause = false;
    }

    public void StartExecute (LinkPath path, System.Action on_arrived, SpeedValueDelegate speed_value_delegate)
    {
        _current_execute_path = path;
        _on_arrived = on_arrived;
        _speed_value_delegate = speed_value_delegate;
        _is_arrived = false;
        _next_path_point_index = 0;

        var path_point_list = _current_execute_path.PathPointList;
        if (path_point_list.Count > 0)
        {
            Utils.SetPosition(transform, path_point_list[0].position);
            ++_next_path_point_index;
        }
    }

    private void update_execute (float dt)
    {
        var path_point_list = _current_execute_path.PathPointList;
        while (dt > 0)
            dt = move(path_point_list, dt);

        if (_is_arrived)
            _on_arrived?.Invoke();
    }

    private float move (List<Transform> path_point_list, float dt)
    {
        if (_next_path_point_index < path_point_list.Count)
        {
            var next_path_point = path_point_list[_next_path_point_index];
            var current_position = this.transform.position;
            var target_positioin = next_path_point.position;
            var distance = Vector2.Distance(current_position, target_positioin);
            var speed = _speed_value_delegate == null ? 1.0f : _speed_value_delegate.Invoke();
            var move_distance = speed * dt;
            if (move_distance <= distance)
            {
                Utils.SetPosition(transform, Vector2.MoveTowards(current_position, target_positioin, move_distance));
                return 0;
            }
            else
            {
                Utils.SetPosition(transform, target_positioin);
                ++_next_path_point_index;
                return (move_distance - distance) / speed;
            }
        }
        else
        {
            _is_arrived = true;
            return 0;
        }
    }
}
