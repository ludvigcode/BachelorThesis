using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quadtree))]
public class QuadtreeEditor : Editor {

    public override void OnInspectorGUI() {
        Quadtree qt = (Quadtree)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Subdivide")) {
            subdivide(qt);
        }

        if (GUILayout.Button("Simplify")) {
            simplify();
        }
        GUILayout.EndHorizontal();
    }

    public void subdivide(Quadtree qt) {
        if (!qt.root) {
            GameObject obj = new GameObject();
            obj.transform.parent = qt.transform;
            obj.transform.position = new Vector3(qt.transform.position.x, qt.transform.position.y, qt.transform.position.z);
            qt.root = obj.AddComponent<QuadtreeNode>();
            qt.root.subdivde(qt);
        }
    }

    public void simplify() {

    }

    private void OnSceneGUI() {
        Quadtree qt = (Quadtree)target;

        if (!qt.root) {
            return;
        }

        display_lines(qt.root);
    }

    private void display_lines(QuadtreeNode node) {
        
    }
}
