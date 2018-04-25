using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDLOD : MonoBehaviour {

    public SceneGrid grid;

    private Camera _cam;

    private void Start() {
        _cam = GetComponent<Camera>();
    }

    private void Update() {
        grid.set_dlods(_cam);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10.0f);
    }
}