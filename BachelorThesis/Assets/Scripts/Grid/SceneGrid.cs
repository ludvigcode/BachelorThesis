using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGrid : MonoBehaviour {

    public bool save_images = false;
    public string folderpath = null;
    public int triangle_limit = 1000;
    public int width = 800;
    public int height = 600;
    public int size = 10;
    public float spread = 10.0f;
    public float y_height = 2.0f;
    public GameObject grid_obj = null;
    public GridNode[,] grid;

    public bool is_initialized = false;

    public void init() {
        if (is_initialized) {
            DestroyImmediate(grid_obj);
        }

        grid_obj = new GameObject();
        grid_obj.transform.parent = transform;
        grid = new GridNode[size + 1, size + 1];

        for (int x = 0; x <= size; ++x) {
            for (int z = 0; z <= size; ++z) {
                GameObject obj = new GameObject("node_" + x + "_" + z);
                obj.transform.position = new Vector3(x * spread, transform.position.y, z * spread);
                obj.transform.parent = grid_obj.transform;

                GridNode node = obj.AddComponent<GridNode>();
                grid[x, z] = node;
                grid[x, z].add_frustums();
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

        // Check direction.
        float angle = Vector3.SignedAngle(camera.transform.forward, Vector3.forward, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            int x_pos = Mathf.Clamp((int)(camera.transform.position.x / spread), 0, size + 1);
            int z_pos = Mathf.Clamp((int)(camera.transform.position.z / spread), 0, size + 1);

            if (angle > 0.0f) {
                grid[x_pos, z_pos].apply_dlods(Direction.NORTH, Direction.EAST);
            } else {
                grid[x_pos, z_pos].apply_dlods(Direction.NORTH, Direction.WEST);
            }

            return;
        }

        angle = Vector3.SignedAngle(camera.transform.forward, Vector3.right, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            int x_pos = Mathf.Clamp((int)(camera.transform.position.x / spread), 0, size + 1);
            int z_pos = Mathf.Clamp((int)(camera.transform.position.z / spread), 0, size + 1);

            if (angle > 0.0f) {
                grid[x_pos, z_pos].apply_dlods(Direction.EAST, Direction.SOUTH);
            } else {
                grid[x_pos, z_pos].apply_dlods(Direction.EAST, Direction.NORTH);
            }

            return;
        }

        angle = Vector3.SignedAngle(camera.transform.forward, Vector3.back, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            int x_pos = Mathf.Clamp((int)(camera.transform.position.x / spread), 0, size + 1);
            int z_pos = Mathf.Clamp((int)(camera.transform.position.z / spread), 0, size + 1);

            if (angle > 0.0f) {
                grid[x_pos, z_pos].apply_dlods(Direction.SOUTH, Direction.EAST);
            } else {
                grid[x_pos, z_pos].apply_dlods(Direction.SOUTH, Direction.WEST);
            }

            return;
        }

        angle = Vector3.SignedAngle(camera.transform.forward, Vector3.left, camera.transform.up);
        if (Mathf.Abs(angle) <= 45.0f) {

            int x_pos = Mathf.Clamp((int)(camera.transform.position.x / spread), 0, size + 1);
            int z_pos = Mathf.Clamp((int)(camera.transform.position.z / spread), 0, size + 1);

            if (angle > 0.0f) {
                grid[x_pos, z_pos].apply_dlods(Direction.WEST, Direction.NORTH);
            } else {
                grid[x_pos, z_pos].apply_dlods(Direction.WEST, Direction.SOUTH);
            }

            return;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;

        if (GridNode.active_1) {
            Gizmos.DrawLine(GridNode.active_1.transform.position, GridNode.active_1.transform.position + GridNode.active_1.transform.forward);
        }

        if (GridNode.active_2) {
            Gizmos.DrawLine(GridNode.active_2.transform.position, GridNode.active_2.transform.position + GridNode.active_2.transform.forward);
        }
    }
}
