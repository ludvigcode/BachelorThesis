using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    NORTH = 0,
    EAST,
    SOUTH,
    WEST
}

public class GridNode : MonoBehaviour {
    private FrustumNode[] _frustums = null;

    public void add_frustums() {
        add_frustum(Direction.NORTH);
        add_frustum(Direction.EAST);
        add_frustum(Direction.SOUTH);
        add_frustum(Direction.WEST);
    }

    public void add_frustum(Direction dir) {
        if (_frustums == null) {
            _frustums = new FrustumNode[4];
            for (int i = 0; i < 4; ++i) {
                _frustums[i] = null;
            }
        }

        if (!_frustums[(int)dir]) {
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
}
