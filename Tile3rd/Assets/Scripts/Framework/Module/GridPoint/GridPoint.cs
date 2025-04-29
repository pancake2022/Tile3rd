using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPoint : CSFramework.CSBehaviour
{
    public Vector2 Size;
    public Transform LeftDown;
    public Transform RightUp;
    private Vector2 _left_down_position;
    private Vector2 _all_grid_size;
    private Vector2 _grid_cell_size;

    private void Awake() 
    {
        Size = new Vector2(Mathf.Max(1, Size.x), Mathf.Max(1, Size.y));

        _left_down_position = LeftDown.position;
        var right_up_position = RightUp.position;

        _all_grid_size = new Vector2(right_up_position.x - _left_down_position.x, right_up_position.y - _left_down_position.y);

        _grid_cell_size = new Vector2(_all_grid_size.x / Size.x, _all_grid_size.y / Size.y);
    }

    public Vector2 PointToPosition (Vector2Int point)
    {
        var clamp_point = new Vector2(Mathf.Clamp(point.x, 0, Size.x), Mathf.Clamp(point.y, 0, Size.y));
        return new Vector2(
            _left_down_position.x + _grid_cell_size.x * (clamp_point.x + 0.5f), 
            _left_down_position.y + _grid_cell_size.y * (clamp_point.y + 0.5f)
        );
    }

    public bool PositionToPoint (Vector2 position, out Vector2Int point)
    {
        position = position - _left_down_position;
        var rect = new Rect(Vector2Int.zero, _all_grid_size);
        if (rect.Contains(position))
        {
            point = new Vector2Int(
                Mathf.FloorToInt(position.x / _grid_cell_size.x),
                Mathf.FloorToInt(position.y / _grid_cell_size.y)
            );
            // Debug.LogError(string.Format("PositionToPoint: {0} -> {1} true", position, point));
            return true;
        }
        else
        {
            point = Vector2Int.zero;
            // Debug.LogError(string.Format("PositionToPoint: {0} -> {1} false", position, point));
            return false;
        }
    }
}
