using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumNode : MonoBehaviour {
    public Camera frustum = null;
    public GridNode parent = null;
    public FrustumNode next_node = null;
    public FrustumNode prev_node = null;
    public bool has_generated_dlods = false;

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

    public int calc_triangles() {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(frustum);

        DLODGroup[] dlods = _get_dlods_within_frustum();
        
        int num_vertices = 0;

        foreach (DLODGroup dlod in dlods) {
            MeshFilter mesh_filter = dlod.dlods[0].GetComponent<MeshFilter>();
            num_vertices += mesh_filter.sharedMesh.triangles.Length;
        }

        return num_vertices;
    }

    public void generate_dlod_table(int max_triangles, int width, int height) {
        DLODGroup[] dlods = _get_dlods_within_frustum();
        if (dlods.Length == 0) {
            return;
        }

        foreach (DLODGroup dlod in dlods) {
            dlod.set_to_original();
        }

        Texture2D reference = _take_screen_shoot(width, height);
        int triangles = calc_triangles();

        while (triangles > max_triangles) {
            Pair<DLODGroup, float>[] dlod_ssim = new Pair<DLODGroup, float>[dlods.Length];
            for (int i = 0; i < dlods.Length; ++i) {
                dlod_ssim[i] = new Pair<DLODGroup, float>(dlods[i], 1.0f);
            }

            foreach (Pair<DLODGroup, float> dlod in dlod_ssim) {
                // If false we, skip. The mesh will be culled.
                if (!dlod.first.try_to_lower()) {
                    dlod.second = -1.0f;
                    continue;
                }

                Texture2D tex = _take_screen_shoot(width, height);
                float mssim = SSIM.compute_mssim_textures(reference, tex, width, height);
                float lowered_triangles = dlod.first.get_active_dlod_mesh_filter().sharedMesh.triangles.Length / 3.0f;
                dlod.first.try_to_higher();
                float higered_triangles = dlod.first.get_active_dlod_mesh_filter().sharedMesh.triangles.Length / 3.0f;
                float triangle_value = 1 - (lowered_triangles / higered_triangles);
                dlod.second = (mssim + triangle_value) / 2.0f;
            }

            for (int i = 0; i < dlods.Length; ++i) {

            }


            triangles = calc_triangles();

            // TMP.
            break;
        }


    }

    private Texture2D _take_screen_shoot(int width, int height) {
        RenderTexture tex = new RenderTexture(width, height, 16);
        Texture2D screen_shot = new Texture2D(width, height, TextureFormat.RGB24, false);

        frustum.targetTexture = tex;
        frustum.Render();

        RenderTexture.active = tex;

        screen_shot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        RenderTexture.active = null;
        frustum.targetTexture = null;
        DestroyImmediate(tex);

        return screen_shot;
    }

    private DLODGroup[] _get_dlods_within_frustum() {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(frustum);

        DLODGroup[] dlods = FindObjectsOfType<DLODGroup>();
        List<DLODGroup> return_dlods = new List<DLODGroup>();

        foreach (DLODGroup dlod in dlods) {
            BoxCollider collider = dlod.GetComponent<BoxCollider>();
            if (GeometryUtility.TestPlanesAABB(planes, collider.bounds)) {
                return_dlods.Add(dlod);
            }
        }

        return return_dlods.ToArray();
    }
}
