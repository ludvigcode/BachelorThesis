using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLODGroup : MonoBehaviour {
    public List<GameObject> dlods;
    private int active_version = -1;

    public void activate(int version) {
        if (active_version != -1) {
            dlods[active_version].SetActive(false);
        }

        dlods[version].SetActive(true);
        active_version = version;
    }
}
