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

    private const float directionScale = 0.5f;
    #endregion

    #region Public Functions
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        spline = target as BezierSpline;
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }

        EditorGUILayout.LabelField("Path", EditorStyles.boldLabel);
        line_color = EditorGUILayout.ColorField("Ray Color", line_color);
        handle_color = EditorGUILayout.ColorField("Sphere Color", handle_color);
        label_color = EditorGUILayout.ColorField("Label Color", label_color);
        handle_size = EditorGUILayout.FloatField("Sphere Radius", handle_size);

        if (GUILayout.Button("Create node"))
        {

        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Points", EditorStyles.boldLabel);
        point_radius = EditorGUILayout.FloatField("Point Radius", point_radius);
        points = EditorGUILayout.IntField("Points per line", points);

        if (GUILayout.Button("Generate points"))
        {

        }
    }

    #endregion

    #region Private Functions
    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for(int i = 1; i < spline.points.Length; i += 3)
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
        
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        //Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline.points[index]);
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotation);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline.points[index] = handleTransform.InverseTransformPoint(point);
        }
        return point;
    }

    
    #endregion
}
