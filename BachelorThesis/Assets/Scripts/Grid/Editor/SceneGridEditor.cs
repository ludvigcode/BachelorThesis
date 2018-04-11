using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneGrid))]
public class SceneGridEditor : Editor {
    public override void OnInspectorGUI() {
        SceneGrid sg = (SceneGrid)target;

        sg.triangle_limit = EditorGUILayout.IntField(sg.triangle_limit);
        sg.size = EditorGUILayout.IntField(sg.size);
        sg.spread = EditorGUILayout.FloatField(sg.spread);

        if (GUILayout.Button("Init")) {
            sg.init();
        }
    }
}
