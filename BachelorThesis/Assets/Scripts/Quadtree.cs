using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree : MonoBehaviour {
    private void Start() {
        GameObject obj = new GameObject();
        obj.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        QuadtreeNode root = obj.AddComponent<QuadtreeNode>();
        root.subdivde(0, 4);
    }
}
