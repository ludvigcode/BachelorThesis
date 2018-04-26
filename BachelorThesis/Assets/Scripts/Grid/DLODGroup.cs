using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLODGroup : MonoBehaviour {
    public List<GameObject> dlods;
    private int active_version = -1;

    public Bounds get_bounds() {
        return dlods[0].GetComponent<MeshRenderer>().bounds;
    }

    public int get_active_version() {
        return active_version;
    }

    public int num_dlod_versions() {
        return dlods.Count;
    }

    public bool is_culled() {
        if (active_version == -1) {
            return true;
        }

        return false;
    }

    public MeshFilter get_active_dlod_mesh_filter() {
        if (active_version == -1) {
            return null;
        }

        return dlods[active_version].GetComponent<MeshFilter>();
    }

    public bool try_to_lower() {
        if (active_version == -1) {
            return false;
        }

        int version = active_version + 1;

        if (version > dlods.Count) {
            return false;
        }

        if (version == dlods.Count) {
            dlods[active_version].SetActive(false);
            active_version = -1;
            return true;
        }

        dlods[active_version].SetActive(false);
        dlods[version].SetActive(true);
        active_version = version;

        return true;
    }

    public bool try_to_higher() {
        int version = active_version;
        if (version == -1) {
            version = dlods.Count - 1;
            dlods[version].SetActive(true);
            active_version = version;
            return true;
        }

        version -= 1;

        if (version >= 0) {
            dlods[active_version].SetActive(false);
            dlods[version].SetActive(true);
            active_version = version;
            return true;
        }

        return false;
    }

    public void set_to_original() {
        if (dlods.Count == 0) {
            return;
        }

        for (int i = 1; i < dlods.Count; ++i) {
            dlods[i].SetActive(false);
        }

        dlods[0].SetActive(true);
        active_version = 0;
    }

    public void cull() {
        if (active_version != -1) {
            dlods[active_version].SetActive(false);
        }

        active_version = -1;
    }

    public void activate(int version) {
        if (dlods.Count == 0) {
            return;
        }

        if (active_version != -1) {
            dlods[active_version].SetActive(false);
        }

        if (version != -1) {
            dlods[version].SetActive(true);
        }

        active_version = version;
    }

    private void Start() {
        foreach (GameObject go in dlods) {
            go.SetActive(false);
        }

        active_version = -1;
    }
}
