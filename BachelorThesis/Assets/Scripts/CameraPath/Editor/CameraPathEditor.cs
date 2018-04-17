using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class CameraPathEditor : Editor
{
    #region Public Variables
    public Color line_color = Color.white;
    public Color handle_color = Color.white;
    public Color label_color = Color.green;
    public float handle_size = 2.0f;

    public float point_radius = 1.0f;
    public int points = 2;
    #endregion

    #region Private Variables
    private BezierSpline spline;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private GUIStyle style = new GUIStyle();

    private const int stepsPerCurve = 10;
    private const float directionScale = 0.5f;
    private const int lineSteps = 10;

    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private int selectedIndex = -1;

    private static Color[] modeColor =
    {
        Color.white,
        Color.yellow,
        Color.cyan
    };
    #endregion

    #region Public Functions
    public override void OnInspectorGUI()
    {
        spline = target as BezierSpline;
        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Toggle Loop");
            EditorUtility.SetDirty(spline);
            spline.Loop = loop;
        }
        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
        {
            DrawSelectedPointInspector();
        }
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }

        //EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
        //line_color = EditorGUILayout.ColorField("Ray Color", line_color);
        //handle_color = EditorGUILayout.ColorField("Sphere Color", handle_color);
        //label_color = EditorGUILayout.ColorField("Label Color", label_color);
        //handle_size = EditorGUILayout.FloatField("Sphere Radius", handle_size);

        //if (GUILayout.Button("Create node"))
        //{

        //}

        //EditorGUILayout.Space();

        //EditorGUILayout.LabelField("Points", EditorStyles.boldLabel);
        //point_radius = EditorGUILayout.FloatField("Point Radius", point_radius);
        //points = EditorGUILayout.IntField("Points per line", points);

        //if (GUILayout.Button("Generate points"))
        //{

        //}
    }

    #endregion

    #region Private Functions
    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.white;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);


            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
        ShowDirections();

    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.GetControlPoint(index));
        float size = HandleUtility.GetHandleSize(point);
        if(index == 0)
        {
            size *= 2f;
        }
        Handles.color = modeColor[(int)spline.GetControlPointMode(index)];
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
            }
        }
        return point;
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline.GetControlPoint(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.SetControlPoint(selectedIndex, point);
        }
        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }
    }
    #endregion
}
