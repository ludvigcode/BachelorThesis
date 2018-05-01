using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraPath))]
public class CameraPathEditor : Editor {

    #region Private Variables
    private GUIStyle _style = new GUIStyle();
    #endregion

    #region Public Functions
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
        cp.loop = EditorGUILayout.Toggle("Loop Path", cp.loop);

        EditorGUILayout.LabelField("Image Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        cp.save_images = EditorGUILayout.Toggle("Save Images", cp.save_images);

        if (cp.save_images) {
            cp.folder_path = EditorGUILayout.TextField("Folderpath", cp.folder_path);
            cp.ímg_width = EditorGUILayout.IntField("Image Width", cp.ímg_width);
            cp.img_height = EditorGUILayout.IntField("Image Height", cp.img_height);
        }
    }
    #endregion

    #region Private Functions
    private void OnSceneGUI() {
        // CameraPath cp = (CameraPath)target;
        // 
        // _style.normal.textColor = Color.green;
        // _style.fontSize = 8;

        //for (int i = 1; i < cp.point_array.Count; i++) {
        //    Vector3 pos = cp.point_array[i].transform.position;
        //    Quaternion rot = cp.point_array[i].transform.rotation;
        //    Vector3 scale;
        //    scale.x = scale.y = scale.z = cp.handle_size;
        //    Handles.DrawWireCube(pos, scale);
        //    Handles.ArrowHandleCap(0, pos, rot, cp.handle_size, EventType.Repaint);
        //    pos.y += cp.handle_size + 1.0f;
        //    Handles.Label(pos, "" + i, _style);
        //}
    }
    #endregion
}
