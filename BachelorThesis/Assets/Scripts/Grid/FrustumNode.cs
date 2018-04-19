using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrustumNode : MonoBehaviour {
    public Camera frustum = null;
    public DLODTable table = null;
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
        DLODGroup[] dlods = _get_dlods_within_frustum();
        int num_vertices = 0;
        foreach (DLODGroup dlod in dlods) {
            if (!dlod.is_culled()) {
                num_vertices += dlod.get_active_dlod_mesh_filter().sharedMesh.triangles.Length / 3;
                
            }
        }

        return num_vertices;
    }

    public void generate_dlod_table(int max_triangles, int width, int height, bool save_file, string folderpath) {
        DLODGroup[] dlods = _get_dlods_within_frustum();
        if (dlods.Length == 0) {
            return;
        }

        foreach (DLODGroup dlod in dlods) {
            dlod.set_to_original();
        }

        int triangles = calc_triangles();

        if (save_file) {
            Directory.CreateDirectory(folderpath + "/" + gameObject.name);
        }

        int itr = 0;
        while (triangles > max_triangles) {
            Texture2D reference = _take_screen_shoot(width, height);

            Pair<DLODGroup, float>[] dlod_ssim = new Pair<DLODGroup, float>[dlods.Length];
            for (int i = 0; i < dlods.Length; ++i) {
                dlod_ssim[i] = new Pair<DLODGroup, float>(dlods[i], 1.0f);

            }

            string folder_path = "";
            if (save_file) {
                folder_path = folderpath + "/" + gameObject.name + "/itr_" + itr.ToString();
                Directory.CreateDirectory(folder_path);
            }

            int counter = 0;
            foreach (Pair<DLODGroup, float> dlod in dlod_ssim) {
                // If false we, skip. The mesh will be culled.
                if (!dlod.first.try_to_lower()) {
                    dlod.second = -1.0f;
                    continue;
                }

                Texture2D tex = _take_screen_shoot(width, height);
                float mssim = SSIM.compute_mssim_textures(reference, tex, width, height);
                dlod.second = mssim;

                if (save_file) {
                    string dlod_table = "";
                    dlod_table += "SSIM: " + mssim.ToString() + "\n\n";

                    foreach (DLODGroup d in dlods) {
                        dlod_table += d.gameObject.name;
                        dlod_table += ": ";
                        dlod_table += d.get_active_version().ToString();
                        dlod_table += "\n";
                    }

                    File.WriteAllText(folder_path + "/" +  gameObject.name + "_itr_" + itr.ToString() + "_" + counter.ToString() + ".txt", dlod_table);
                    File.WriteAllBytes(folder_path + "/" + gameObject.name + "_itr_" + itr.ToString() + "_" + counter.ToString() + ".png", tex.EncodeToPNG());
                }

                dlod.first.try_to_higher();

                ++counter;
            }

            int index = -1;
            float val = 0.0f;
            for (int i = 0; i < dlods.Length; ++i) {
                if (dlod_ssim[i].second > val) {
                    index = i;
                    val = dlod_ssim[i].second;
                }
            }

            if (index >= 0) {
                if (dlod_ssim[index].first.try_to_lower()) {
                    triangles = calc_triangles();
                }
            }

            ++itr;
        }

        table = GetComponent<DLODTable>();
        if (!table) {
            table = gameObject.AddComponent<DLODTable>();
        }

        if (save_file) {
            string dlod_table = "";
            foreach (DLODGroup d in dlods) {
                dlod_table += d.gameObject.name;
                dlod_table += ": ";
                dlod_table += d.get_active_version().ToString();
                dlod_table += "\n";
            }

            Texture2D tex = _take_screen_shoot(width, height);

            File.WriteAllText(folderpath + "/" + gameObject.name + "/" + gameObject.name + "_golden.txt", dlod_table);
            File.WriteAllBytes(folderpath + "/" + gameObject.name + "/" + gameObject.name + "_golden.png", tex.EncodeToPNG());
        }

        foreach (DLODGroup dlod in dlods) {
            table.push_back(dlod, dlod.get_active_version());
            dlod.set_to_original();
        }

        triangles = calc_triangles();
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
