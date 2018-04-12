using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour {

    private void Start () {
		
	}

    private void Update() {
        calc_dlods();
    }

    public void calc_dlods() {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());

        DLODGroup[] dlods = FindObjectsOfType<DLODGroup>();
        foreach(DLODGroup dlod in dlods) {
            BoxCollider collider = dlod.GetComponent<BoxCollider>();
            if (GeometryUtility.TestPlanesAABB(planes, collider.bounds)) {
                dlod.dlods[0].SetActive(true);
            } else {
                dlod.dlods[0].SetActive(false);
            }
        }
    }
}

