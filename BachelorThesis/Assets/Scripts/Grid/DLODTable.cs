using System;
using System.Collections.Generic;
using UnityEngine;

public class DLODTable : MonoBehaviour {
    [Serializable]
    public struct DLODVersion {
        public DLODGroup dlod;
        public int version;
    }

    public List<DLODVersion> dlods = null;

    public void push_back(DLODGroup dlod, int version) {
        if (dlods == null) {
            dlods = new List<DLODVersion>();
        }

        DLODVersion dlod_version = new DLODVersion();
        dlod_version.dlod = dlod;
        dlod_version.version = version;
        dlods.Add(dlod_version);
    }

    public void apply() {
        foreach (DLODVersion dlod in dlods) {
            dlod.dlod.activate(dlod.version);
        }
    }
}
