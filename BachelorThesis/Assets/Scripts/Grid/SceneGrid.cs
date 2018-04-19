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
    public float threshold = 2.0f;
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
            grid[0, 0].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[0, size].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[size, 0].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[size, size].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[0, size / 2].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[size, size / 2].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[size / 2, 0].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[size / 2, size].generate_dlod_table((Direction)i, triangle_limit, width, height);
            grid[size / 2, size / 2].generate_dlod_table((Direction)i, triangle_limit, width, height);
        }

        is_initialized = true;
    }

    public void subdivide() {
        bool north = false;
        bool east = false;
        bool south = false;
        bool west = false;
        int counter = 0;

        while ((!north || !east || !south || !west) && counter < 100) {
            north = true;
            east = true;
            south = true;
            west = true;

            for (int x = 0; x <= size; ++x) {
                int prev_z = -1;
                int prev_num_vertices = -1;
                for (int z = 0; z <= size; ++z) {
                    int vertices = grid[x, z].get_num_vertices(Direction.NORTH);
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 1.0f / threshold || diff > 1.0f * threshold) {
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
                int prev_x = -1;
                int prev_num_vertices = -1;
                for (int x = 0; x <= size; ++x) {
                    int vertices = grid[x, z].get_num_vertices(Direction.EAST);
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 1.0f / threshold || diff > 1.0f * threshold) {
                                int new_x = prev_x + ((x - prev_x) / 2);
                                grid[new_x, z].add_frustums();
                                east = false;
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
                for (int z = size; z >= 0; --z) {
                    int vertices = grid[x, z].get_num_vertices(Direction.SOUTH);
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 1.0f / threshold || diff > 1.0f * threshold) {
                                int new_z = z + ((prev_z - z) / 2);
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
                int prev_x = -1;
                int prev_num_vertices = -1;
                for (int x = size; x >= 0; --x) {
                    int vertices = grid[x, z].get_num_vertices(Direction.WEST);
                    if (vertices > 0) {
                        if (prev_num_vertices != -1) {
                            float diff = prev_num_vertices / vertices;
                            if (diff < 1.0f / threshold || diff > 1.0f * threshold) {
                                int new_x = x + ((prev_x - x) / 2);
                                grid[new_x, z].add_frustums();
                                west = false;
                            }
                        }

                        prev_x = x;
                        prev_num_vertices = vertices;
                    }
                }
            }

            ++counter;
        }
    }
}