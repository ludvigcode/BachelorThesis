using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadtreeNode : MonoBehaviour {
    public float size = 400.0f;
    bool is_leaf = true;
    public QuadtreeNode parent = null;
    public QuadtreeNode top_left = null;
    public QuadtreeNode top_right = null;
    public QuadtreeNode bottom_left = null;
    public QuadtreeNode bottom_right = null;

    public void subdivde(int depth, int max_depth) {
        GameObject obj1 = new GameObject();
        obj1.transform.parent = transform;
        obj1.transform.position = new Vector3(transform.position.x + size, transform.position.y, transform.position.z - size);
        top_left = obj1.AddComponent<QuadtreeNode>();
        top_left.parent = this;
        top_left.size = size / 2;

        GameObject obj2 = new GameObject();
        obj2.transform.parent = transform;
        obj2.transform.position = new Vector3(transform.position.x + size, transform.position.y, transform.position.z + size);
        top_right = obj2.AddComponent<QuadtreeNode>();
        top_right.parent = this;
        top_right.size = size / 2;

        GameObject obj3 = new GameObject();
        obj3.transform.parent = transform;
        obj3.transform.position = new Vector3(transform.position.x - size, transform.position.y, transform.position.z - size);
        bottom_left = obj3.AddComponent<QuadtreeNode>();
        bottom_left.parent = this;
        bottom_left.size = size / 2;

        GameObject obj4 = new GameObject();
        obj4.transform.parent = transform;
        obj4.transform.position = new Vector3(transform.position.x - size, transform.position.y, transform.position.z + size);
        bottom_right = obj4.AddComponent<QuadtreeNode>();
        bottom_right.parent = this;
        bottom_right.size = size / 2;

        is_leaf = false;

        if (depth <= max_depth) {
            top_left.subdivde(depth + 1, max_depth);
            top_right.subdivde(depth + 1, max_depth);
            bottom_left.subdivde(depth + 1, max_depth);
            bottom_right.subdivde(depth + 1, max_depth);

        }
    }

    private void Update() {
        if (!is_leaf) {
            Vector3 p1 = new Vector3(top_left.transform.position.x + size, top_left.transform.position.y, top_left.transform.position.z - size);
            Vector3 p2 = new Vector3(top_right.transform.position.x + size, top_right.transform.position.y, top_right.transform.position.z + size);
            Vector3 p3 = new Vector3(bottom_left.transform.position.x - size, bottom_left.transform.position.y, bottom_left.transform.position.z - size);
            Vector3 p4 = new Vector3(bottom_right.transform.position.x - size, bottom_right.transform.position.y, bottom_right.transform.position.z + size);

            Debug.DrawLine(p1, p2, Color.red);
            Debug.DrawLine(p1, p3, Color.red);
            Debug.DrawLine(p4, p3, Color.red);
            Debug.DrawLine(p4, p2, Color.red);
        }
    }
}

