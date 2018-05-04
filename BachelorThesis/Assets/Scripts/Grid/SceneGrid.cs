using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGrid : MonoBehaviour {

    public bool save_images = false;
    public string folderpath = null;
    public int max_dlod_versions = 5;
    public int width = 800;
    public int height = 600;
    public int triangle_limit = 1000;
    public int size = 10;
    public int spread = 10;
    public float y_height = 2.0f;
    public GameObject grid_obj = null;
    public Simplification simp = null;

    [SerializeField]
    public GridNode[,] grid;

    public bool is_initialized = false;

    public void simplify() {
        simp = new Simplification();
        _create_dlods();
    }

    public void init() {
        grid_obj = new GameObject();
        grid_obj.transform.parent = transform;
        grid = new GridNode[size + 1, size + 1];

        for (int x = 0; x <= size; ++x) {
            for (int z = 0; z <= size; ++z) {
                GameObject obj = new GameObject("node_" + x + "_" + z);
                obj.transform.position = new Vector3(x * spread, y_height, z * spread);
                obj.transform.parent = grid_obj.transform;

                GridNode node = obj.AddComponent<GridNode>();
                node.add_frustums();
                grid[x, z] = node;
            }
        }

        for (int x = 0; x <= size; ++x) {
            for (int z = 0; z <= size; ++z) {
                for (int i = 0; i < 4; ++i) {
                    grid[x, z].generate_dlod_table((Direction)i, triangle_limit, width, height, save_images, folderpath);
                }
            }
        }

        is_initialized = true;
    }

    public void set_dlods(Camera camera) {

        int x_pos = (int)(camera.transform.position.x / spread);
        int z_pos = (int)(camera.transform.position.z / spread);

        DLODGroup[] dlods = GameObject.FindObjectsOfType<DLODGroup>();

        foreach (DLODGroup dlod in dlods) {
            dlod.cull();
        }

        // Check direction.
        float angle = Vector3.SignedAngle(camera.transform.forward, Vector3.forward, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            if (angle < 0.0f) {
                x_pos = Mathf.Clamp(x_pos, 0, size);
                z_pos = Mathf.Clamp(z_pos, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.NORTH, Direction.EAST);
            } else {
                x_pos = Mathf.Clamp(x_pos + 1, 0, size);
                z_pos = Mathf.Clamp(z_pos, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.NORTH, Direction.WEST);
            }

            return;
        }

        angle = Vector3.SignedAngle(camera.transform.forward, Vector3.right, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            if (angle < 0.0f) {
                x_pos = Mathf.Clamp(x_pos, 0, size);
                z_pos = Mathf.Clamp(z_pos + 1, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.EAST, Direction.SOUTH);
            } else {
                x_pos = Mathf.Clamp(x_pos, 0, size);
                z_pos = Mathf.Clamp(z_pos, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.EAST, Direction.NORTH);
            }

            return;
        }

        angle = Vector3.SignedAngle(camera.transform.forward, Vector3.back, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            if (angle > 0.0f) {
                x_pos = Mathf.Clamp(x_pos, 0, size);
                z_pos = Mathf.Clamp(z_pos + 1, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.SOUTH, Direction.EAST);
            } else {
                x_pos = Mathf.Clamp(x_pos + 1, 0, size);
                z_pos = Mathf.Clamp(z_pos + 1, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.SOUTH, Direction.WEST);
            }

            return;
        }

        angle = Vector3.SignedAngle(camera.transform.forward, Vector3.left, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            if (angle < 0.0f) {
                x_pos = Mathf.Clamp(x_pos + 1, 0, size);
                z_pos = Mathf.Clamp(z_pos, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.WEST, Direction.NORTH);
            } else {
                x_pos = Mathf.Clamp(x_pos + 1, 0, size);
                z_pos = Mathf.Clamp(z_pos + 1, 0, size);
                grid[x_pos, z_pos].apply_dlods(Direction.WEST, Direction.SOUTH);
            }

            return;
        }
    }

    public void find_nodes() {
        grid = new GridNode[size + 1, size + 1];

        for (int x = 0; x <= size; ++x) {
            for (int z = 0; z <= size; ++z) {
                for (int i = 0; i < 4; ++i) {
                    GameObject obj = GameObject.Find("node_" + x + "_" + z);
                    GridNode node = obj.GetComponent<GridNode>();
                    if (node) {
                        node.find_frustums();
                        grid[x, z] = node;
                    }
                }
            }
        }
    }

    public void remove_cameras() {
        Camera[] cameras = FindObjectsOfType<Camera>();

        foreach (Camera cam in cameras) {
            if (cam != Camera.main) {
                DestroyImmediate(cam);
            }
        }
    }

    private void Start() {
        if (grid == null) {
            find_nodes();
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;

        if (GridNode.active_1) {
            Gizmos.DrawLine(GridNode.active_1.transform.position, GridNode.active_1.transform.position + GridNode.active_1.transform.forward * 10.0f);
        }

        if (GridNode.active_2) {
            Gizmos.DrawLine(GridNode.active_2.transform.position, GridNode.active_2.transform.position + GridNode.active_2.transform.forward * 10.0f);
        }

        is_initialized = true;
    }

    private void _create_dlods() {

        simp.lod_levels = max_dlod_versions;
        simp.create_lod();
    }
}
