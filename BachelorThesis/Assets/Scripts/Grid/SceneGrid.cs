using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGrid : MonoBehaviour {
    public bool save_images = false;
    public string folderpath = "";
    public int triangle_limit = 1000;
    public int size = 10;
    public float spread = 10.0f;
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
            }
        }

        grid[0, 0].add_frustums();
        grid[0, size].add_frustums();
        grid[size, 0].add_frustums();
        grid[size, size].add_frustums();
        grid[0, size / 2].add_frustums();
        grid[size, size / 2].add_frustums();
        grid[size / 2, 0].add_frustums();
        grid[size / 2, size].add_frustums();
        grid[size / 2, size / 2].add_frustums();

        for (int i = 0; i < 4; ++i) {
            grid[0, 0].add_frustums();
            grid[0, size].se();
            grid[size, 0].add_frustums();
            grid[size, size].add_frustums();
            grid[0, size / 2].add_frustums();
            grid[size, size / 2].add_frustums();
            grid[size / 2, 0].add_frustums();
            grid[size / 2, size].add_frustums();
            grid[size / 2, size / 2].add_frustums();
        }


        is_initialized = true;
    }
}
