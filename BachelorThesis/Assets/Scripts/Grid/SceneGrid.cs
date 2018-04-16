using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneGrid : MonoBehaviour {
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

        bool north = false;
        bool east = false;
        bool south = false;
        bool west = false;
        int counter = 0;
        while ((!north || !east || south || west) && counter < 100) {
            north = true;
            east = true;
            south = true;
            west = true;

            for (int x = 0; x <= size; ++x) {
                int prev_z = -1;
                int prev_num_vertices = -1;
                for (int z = 0; z <= size; ++z) {
                    int vertices = (grid[x, z].get_num_vertices(Direction.NORTH));
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 0.75f || diff > 1.5f) {
                                int new_z = prev_z + ((z - prev_z) / 2);
                                grid[x, new_z].add_frustums();
                                north = false;
                            }
                        }

                        prev_z = z;
                        prev_num_vertices = vertices;
                    }
                }
            }

            for (int z = 0; z <= size; ++z) {
                for (int x = 0; x <= size; ++x) {
                    int prev_x = -1;
                    int prev_num_vertices = -1;
                    int vertices = (grid[x, z].get_num_vertices(Direction.WEST));
                    Debug.Log(vertices);
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 0.75f || diff > 1.5f) {
                                int new_z = prev_x + ((x - prev_x) / 2);
                                grid[x, new_z].add_frustums();
                                east = false;
                                Debug.Log("YES!");
                            }
                        }

                        prev_x = x;
                        prev_num_vertices = vertices;
                    }
                }
            }

            for (int x = 0; x <= size; ++x) {
                int prev_z = -1;
                int prev_num_vertices = -1;
                for (int z = size; z <= 0; ++z) {
                    int vertices = (grid[x, z].get_num_vertices(Direction.SOUTH));
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 0.75f || diff > 1.5f) {
                                int new_z = prev_z + ((z - prev_z) / 2);
                                grid[x, new_z].add_frustums();
                                south = false;
                            }
                        }

                        prev_z = z;
                        prev_num_vertices = vertices;
                    }
                }
            }

            for (int z = 0; z <= size; ++z) {
                for (int x = size; x <= 0; ++x) {
                    int prev_x = -1;
                    int prev_num_vertices = -1;
                    int vertices = (grid[x, z].get_num_vertices(Direction.EAST));
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 0.75f || diff > 1.5f) {
                                int new_x = prev_x + ((x - prev_x) / 2);
                                grid[x, new_x].add_frustums();
                                west = false;
                                Debug.Log("YES!2");
                            }
                        }

                        prev_x = x;
                        prev_num_vertices = vertices;
                    }
                }
            }

            ++counter;
        }

        is_initialized = true;
    }
}
