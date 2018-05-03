using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPath))]
public class CameraPathEditor : Editor {

    private GUIStyle _style = new GUIStyle();

    public override void OnInspectorGUI() {
        CameraPath cp = (CameraPath)target;

        EditorGUILayout.LabelField("Generate Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        cp.frequency = EditorGUILayout.IntField("Points", cp.frequency);
        cp.lookForward = EditorGUILayout.Toggle("Look Forward", cp.lookForward);
        cp.spline = (BezierSpline)EditorGUILayout.ObjectField("Spline", cp.spline, typeof(BezierSpline), true);

        cp.handle_size = EditorGUILayout.FloatField("Handle Size", cp.handle_size);

        if (GUILayout.Button("Generate camera points")) {
            cp.generate_points();
        }

        if (GUILayout.Button("Clear")) {
            cp.clear();
        }

        EditorGUILayout.LabelField("Runtime Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        cp.steptime = EditorGUILayout.FloatField("Step Time", cp.steptime);

        EditorGUILayout.LabelField("Image Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        cp.save_images = EditorGUILayout.Toggle("Save Images", cp.save_images);

        if (cp.save_images) {
            cp.folder_path = EditorGUILayout.TextField("Folderpath", cp.folder_path);
            cp.ímg_width = EditorGUILayout.IntField("Image Width", cp.ímg_width);
            cp.img_height = EditorGUILayout.IntField("Image Height", cp.img_height);
        }
    }
}
