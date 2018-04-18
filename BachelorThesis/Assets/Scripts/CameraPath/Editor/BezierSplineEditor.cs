using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierSplineEditor : Editor
{

    #region Private Variables
    private BezierSpline _spline;
    private Transform _handle_transform;
    private Quaternion _handle_rotation;

    private const int _steps_per_curve = 10;
    private const float _direction_scale = 0.5f;
    private const int _line_steps = 10;

    private const float _handle_size = 0.04f;
    private const float _pick_size = 0.06f;
    private int _selected_index = -1;

    private static Color[] _modeColor =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };
    #endregion

    #region Public Functions
    public override void OnInspectorGUI()
    {
        _spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", _spline.loop);
        bool force = EditorGUILayout.Toggle("Force Y", _spline.force_Y);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Toggle Loop");
            EditorUtility.SetDirty(_spline);
            _spline.loop = loop;
            _spline.force_Y = force;
        }
        if (_selected_index >= 0 && _selected_index < _spline.control_point_count)
        {
            _draw_selected_point_inspector();
        }
        if (GUILayout.Button("Init"))
        {
            _spline.init();
        }
        if (_spline.force_Y)
        {
            _spline._forced_Y = EditorGUILayout.FloatField("Y Translation", _spline._forced_Y);
            if (GUILayout.Button("Force Y Translation"))
            {
                _spline.force_y_translation();
            }
        }
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(_spline, "Add Curve");
            _spline.add_curve();
            EditorUtility.SetDirty(_spline);
        }
    }

    #endregion

    #region Private Functions
    private void OnSceneGUI()
    {
        _spline = target as BezierSpline;
        if (!_spline.is_initialized())
        {
            return;
        }

        _handle_transform = _spline.transform;
        _handle_rotation = Tools.pivotRotation == PivotRotation.Local ? _handle_transform.rotation : Quaternion.identity;

        Vector3 p0 = _show_point(0);
        for (int i = 1; i < _spline.control_point_count; i += 3)
        {
            Vector3 p1 = _show_point(i);
            Vector3 p2 = _show_point(i + 1);
            Vector3 p3 = _show_point(i + 2);

            Handles.color = Color.white;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);


            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        _show_directions();

    }

    private void _show_directions()
    {
        Handles.color = Color.green;
        Vector3 point = _spline.get_point(0f);
        Handles.DrawLine(point, point + _spline.get_direction(0f) * _direction_scale);
        int steps = _steps_per_curve * _spline.curve_count;
        for (int i = 1; i <= steps; i++)
        {
            point = _spline.get_point(i / (float)steps);
            Handles.DrawLine(point, point + _spline.get_direction(i / (float)steps) * _direction_scale);
        }
    }

    private Vector3 _show_point(int index)
    {
        Vector3 point = _handle_transform.TransformPoint(_spline.get_control_point(index));
        float size = HandleUtility.GetHandleSize(point);
        if(index == 0)
        {
            size *= 2f;
        }
        Handles.color = _modeColor[(int)_spline.get_control_point_mode(index)];
        if (Handles.Button(point, _handle_rotation, size * _handle_size, size * _pick_size, Handles.DotHandleCap))
        {
            _selected_index = index;
            Repaint();
        }
        if (_selected_index == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, _handle_rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_spline, "Move Point");
                EditorUtility.SetDirty(_spline);
                _spline.set_control_point(index, _handle_transform.InverseTransformPoint(point));
            }
        }
        return point;
    }

    private void _draw_selected_point_inspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", _spline.get_control_point(_selected_index));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Move Point");
            EditorUtility.SetDirty(_spline);
            _spline.set_control_point(_selected_index, point);
        }
        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", _spline.get_control_point_mode(_selected_index));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(_spline, "Change Point Mode");
            _spline.set_control_point_mode(_selected_index, mode);
            EditorUtility.SetDirty(_spline);
        }
    }
    #endregion
}
