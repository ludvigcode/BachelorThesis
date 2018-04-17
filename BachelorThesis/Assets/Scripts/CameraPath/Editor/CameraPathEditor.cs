using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPath))]
public class CameraPathEditor : Editor {

    public override void OnInspectorGUI()
    {
        CameraPath cp = (CameraPath)target;

        cp.frequency = EditorGUILayout.IntField("Frequency", cp.frequency);
        cp.spline = (BezierSpline)EditorGUILayout.ObjectField("Spline", cp.spline, typeof(BezierSpline), true);

        if (GUILayout.Button("Generate camera points"))
        {
            cp.generate_points();
        }
    }

    private void OnSceneGUI()
    {
        
    }
}
