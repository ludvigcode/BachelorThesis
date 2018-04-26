using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3
}

public class GridNode : MonoBehaviour {

    public static FrustumNode active_1 { get; private set; }
    public static FrustumNode active_2 { get; private set; }

    [SerializeField]
    private FrustumNode[] _frustums = null;

    public void add_frustums() {
        if (_frustums == null) {
            _frustums = new FrustumNode[4];
            for (int i = 0; i < 4; ++i) {
                _frustums[i] = null;
            }
        }

        add_frustum(Direction.NORTH);
        add_frustum(Direction.EAST);
        add_frustum(Direction.SOUTH);
        add_frustum(Direction.WEST);
    }

    public void apply_dlods(Direction dir_1, Direction dir_2) {
        if (_frustums != null) {
            if (_frustums[(int)dir_1]) {
                _frustums[(int)dir_1].apply_dlod_table();
                active_1 = _frustums[(int)dir_1];
            } else {
                active_1 = null;
            }

            if (_frustums[(int)dir_2]) {
                _frustums[(int)dir_2].apply_dlod_table();
                active_2 = _frustums[(int)dir_2];
            } else {
                active_2 = null;
            }
        }
    }

    public FrustumNode get_frustum(Direction dir) {
        if (_frustums != null) {
            return _frustums[(int)dir];
        }

        return null;
    }

    public bool has_frustum(Direction dir) {
        if (_frustums != null) {
            if (_frustums[(int)dir]) {
                return true;
            }
        }

        return false;
    }

    public void set_next_frustum(Direction dir, GridNode node) {
        if (_frustums != null) {
            if (_frustums[(int)dir]) {
                if (node._frustums[(int)dir]) {
                    _frustums[(int)dir].next_node = node._frustums[(int)dir];
                    node._frustums[(int)dir].prev_node = _frustums[(int)dir];
                }
            }
        }
    }

    public void generate_dlod_table(Direction dir, int max_triangles, int width, int height, bool save_file, string folderpath) {
        if (_frustums != null) {
            if (_frustums[(int)dir]) {
                _frustums[(int)dir].generate_dlod_table(max_triangles, width, height, save_file, folderpath);
            }
        }
    }

    public int get_num_vertices(Direction dir) {
        if (_frustums != null) {
            if (_frustums[(int)dir]) {
                return _frustums[(int)dir].calc_triangles();
            }
        }

        return -1;
    }

    public void find_frustums() {
        _frustums = new FrustumNode[4];
        _frustums[0] = null;
        _frustums[1] = null;
        _frustums[2] = null;
        _frustums[3] = null;

        GameObject north = GameObject.Find(gameObject.name + "_north");
        if (north) {
            FrustumNode n = north.GetComponent<FrustumNode>();
            if (n) {
                _frustums[(int)Direction.NORTH] = n;
            }
        }

        GameObject east = GameObject.Find(gameObject.name + "_east");
        if (east) {
            FrustumNode n = east.GetComponent<FrustumNode>();
            if (n) {
                _frustums[(int)Direction.EAST] = n;
            }
        }

        GameObject south = GameObject.Find(gameObject.name + "_south");
        if (south) {
            FrustumNode n = south.GetComponent<FrustumNode>();
            if (n) {
                _frustums[(int)Direction.SOUTH] = n;
            }
        }

        GameObject west = GameObject.Find(gameObject.name + "_west");
        if (west) {
            FrustumNode n = west.GetComponent<FrustumNode>();
            if (n) {
                _frustums[(int)Direction.WEST] = n;
            }
        }
    }

    private void add_frustum(Direction dir) {
        GameObject obj = new GameObject();
        _frustums[(int)dir] = obj.AddComponent<FrustumNode>();

        switch (dir) {
            case Direction.NORTH:
                obj.name = gameObject.name + "_north";
                _frustums[(int)dir].init(this, 0.0f);
                break;
            case Direction.EAST:
                obj.name = gameObject.name + "_east";
                _frustums[(int)dir].init(this, 90.0f);
                break;
            case Direction.SOUTH:
                obj.name = gameObject.name + "_south";
                _frustums[(int)dir].init(this, 180.0f);
                break;
            case Direction.WEST:
                obj.name = gameObject.name + "_west";
                _frustums[(int)dir].init(this, 270.0f);
                break;
            default:
                break;
        }
    }
}
