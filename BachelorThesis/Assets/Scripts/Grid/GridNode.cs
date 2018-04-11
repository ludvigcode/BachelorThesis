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
    private FrustumNode north = null;
    private FrustumNode east = null;
    private FrustumNode south = null;
    private FrustumNode west = null;

    public void add_frustum(Direction dir) {
        if (dir == Direction.NORTH && !north) {
            GameObject obj = new GameObject();
            north = obj.AddComponent<FrustumNode>();
            north.init(this, 0.0f);
            return;
        }

        if (dir == Direction.EAST && !east) {
            GameObject obj = new GameObject();
            east = obj.AddComponent<FrustumNode>();
            east.init(this, 90.0f);
            return;
        }

        if (dir == Direction.SOUTH && !south) {
            GameObject obj = new GameObject();
            south = obj.AddComponent<FrustumNode>();
            south.init(this, 180.0f);
            return;
        }

        if (dir == Direction.WEST && !west) {
            GameObject obj = new GameObject();
            west = obj.AddComponent<FrustumNode>();
            west.init(this, 270.0f);
            return;
        }
    }

    public FrustumNode get_frustum(Direction dir) {
        if (dir == Direction.NORTH && north) {
            return north;
        }

        if (dir == Direction.EAST && east) {
            return east;
        }

        if (dir == Direction.SOUTH && south) {
            return south;
        }

        if (dir == Direction.WEST && west) {
            return west;
        }

        return null;
    }

    public bool has_frustum(Direction dir) {
        if (dir == Direction.NORTH && north) {
            return true;
        }

        if (dir == Direction.EAST && east) {
            return true;
        }

        if (dir == Direction.SOUTH && south) {
            return true;
        }

        if (dir == Direction.WEST && west) {
            return true;
        }

        return false;
    }

    public void set_next_frustum(Direction dir, GridNode node) {
        if (dir == Direction.NORTH && north) {
            north.next_node = node.north;
            node.north.prev_node = north;
            return;
        }

        if (dir == Direction.EAST && east) {
            east.next_node = node.east;
            node.east.prev_node = east;
            return;
        }

        if (dir == Direction.SOUTH && south) {
            south.next_node = node.south;
            node.south.prev_node = south;
            return;
        }

        if (dir == Direction.WEST && west) {
            west.next_node = node.west;
            node.west.prev_node = west;
            return;
        }
    }
}
