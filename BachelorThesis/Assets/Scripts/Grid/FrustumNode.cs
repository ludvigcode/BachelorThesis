using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumNode : MonoBehaviour {
    public Camera frustum = null;
    public GridNode parent = null;
    public FrustumNode next_node = null;
    public FrustumNode prev_node = null;

    public void init(GridNode parent, float rotation) {
        this.parent = parent;
        transform.parent = parent.transform;
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        frustum = gameObject.AddComponent<Camera>();
        frustum.fieldOfView = 54.56f;
        frustum.nearClipPlane = 1.0f;
        frustum.farClipPlane = 100.0f;
    }

    public int calc_vertices() {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(frustum);

        DLODGroup[] dlods = FindObjectsOfType<DLODGroup>();

        int num_vertices = 0;

        foreach (DLODGroup dlod in dlods) {
            BoxCollider collider = dlod.GetComponent<BoxCollider>();
            if (GeometryUtility.TestPlanesAABB(planes, collider.bounds)) {
                dlod.dlods[0].SetActive(true);
                MeshFilter mesh_filter = dlod.dlods[0].GetComponent<MeshFilter>();
                num_vertices += mesh_filter.sharedMesh.triangles.Length;
            } else {
                dlod.dlods[0].SetActive(false);
            }
        }

        return num_vertices;
    }

    public void generate_dlod_table(int max_triangles) {
        RenderTexture tex = new RenderTexture(800, 800, 16);
        Texture2D screen_shot = new Texture2D(800, 800, TextureFormat.RGB24, false);

        frustum.targetTexture = tex;
        frustum.Render();

        RenderTexture.active = tex;

        screen_shot.ReadPixels(new Rect(0, 0, 800, 800), 0, 0);


        RenderTexture.active = null;
        frustum.targetTexture = null;
        DestroyImmediate(tex);

        byte[] bytes = screen_shot.EncodeToPNG();
        System.IO.File.WriteAllBytes("C:/Users/BTH/Documents/GitHub/lol.png", bytes);

    }
}
