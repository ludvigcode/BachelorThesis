// SimplificationTools.cs
// Fredrik Linde TA15
// Mesh Incremental Decimation Simplification Utility
// Using the UnityMeshSimplifier by Whinarn
// All materials belong to its rightful owner.
// https://github.com/Whinarn/UnityMeshSimplifier

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityMeshSimplifier;

public class RenderingTarget {
    //public GameObject game_object;
    public MeshFilter mesh_filter;
    public List<Mesh> lod_array;

    public RenderingTarget(MeshFilter filter, List<Mesh> mesh_list) {
        // Cache the mesh filter and create mesh array.
        mesh_filter = filter;
        lod_array = new List<Mesh>(mesh_list);

        // Set to highest quality.
        //set_lod(0);
    }

    // Set the rendering target LOD version.
    public void set_lod(int index) {
        set_mesh(lod_array[index]);
    }

    // Set the rendering target mesh.
    public void set_mesh(Mesh mesh) {
        mesh_filter.mesh = mesh;
    }
}

public class Simplification {

    public int lod_levels;
    public Dictionary<int, List<Mesh>> table;

    private List<RenderingTarget> _scene_objects;

    public void create_lod() {
        // Create the dictionary.
        table = new Dictionary<int, List<Mesh>>();

        // Create the a list for rendering targets in scene.
        _scene_objects = new List<RenderingTarget>();

        // Get all mesh filters in the scene.
        MeshFilter[] filters = GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

        // Loop through each mesh filter and create rendering targets from their meshes.
        foreach (MeshFilter filter in filters) {
            // Get the hash code of the shared mesh, the original mesh. This will remain identical throughout the applicaton.
            // filter.mesh returns an instantiated clone of the mesh, which will always produce a unique hash code.
            int key = filter.sharedMesh.GetHashCode();

            // Has this mesh already been processed? 
            if (table.ContainsKey(key)) {
                Debug.Log("Mesh with key " + key.ToString() + " already exists. Using existing LOD meshes...");

                // Since the dictionary already holds the key to this mesh, the LOD versions have 
                // already been generated. Take the references from the mesh list in the dictionary
                // and give them to the new rendering target.
                _scene_objects.Add(new RenderingTarget(filter, table[key]));

            } else {
                Debug.Log("Mesh with key " + key.ToString() + " did not exist. Creating new LOD meshes...");

                // Create new mesh list and genereate LOD versions.
                List<Mesh> array = new List<Mesh>();

                // We require a copy of the shared mesh.
                Mesh meshCopy = GameObject.Instantiate(filter.sharedMesh) as Mesh;

                _generate_lod_versions(meshCopy, array, lod_levels);

                // Store LOD array as value in the dictionary given the original mesh hash code as key.
                table.Add(key, array);

                // Add new rendering target to the scene with LOD version list from dictionary.
                _scene_objects.Add(new RenderingTarget(filter, array));

            }

        }

        _save_lods_to_file();
        _create_dlod_objects();
    }

    private void _generate_lod_versions(Mesh mesh, List<Mesh> list, int lod_count) {
        // The first instance in the LOD array is the default mesh.
        list.Add(mesh);

        // Initialize the mesh simplifier.
        MeshSimplifier simplifier = new MeshSimplifier();

        // Iterate through each LOD version.
        for (int i = 1; i < lod_count; i++) {
            // Initialize the mesh simplifier with the previous LOD version mesh.
            simplifier.Initialize(list[i - 1]);

            // Simplify by a given percentage (0.5 = 50% by default).
            simplifier.SimplifyMesh(0.5f);

            // Get the resulting mesh. 
            Mesh destMesh = simplifier.ToMesh();

            // Set the resulting mesh to the current LOD version.
            list.Add(destMesh);

        }
    }

    private void _save_lods_to_file() {
        foreach (KeyValuePair<int, List<Mesh>> hash in table) {
            string directorypath = "Assets/Meshes/Mesh" + hash.Key.ToString();
            try {
                if (!Directory.Exists(directorypath)) {
                    Directory.CreateDirectory(directorypath);
                }

            } catch (IOException ex) {
                Debug.Log(ex.Message);
            }

            int lod = 0;
            foreach (Mesh mesh in hash.Value) {
                if (lod != 0) {
                    string savepath = directorypath + "/" + hash.Key.ToString() + "_" + lod.ToString() + ".asset";
                    _save_mesh_asset(mesh, savepath);
                }
                lod++;
            }
        }
    }

    private void _save_mesh_asset(Mesh mesh, string path) {
        AssetDatabase.CreateAsset(mesh, path);
    }

    private void _create_dlod_objects() {

        // Go through each scene object (mesh filter).
        for (int i = 0; i < _scene_objects.Count; ++i) {

            // Get parent game object to LOD0. 
            Transform parent = _scene_objects[i].mesh_filter.transform.parent;
            if (parent) {
                DLODGroup group = parent.GetComponent<DLODGroup>();
                if (group) {

                    // Get the shared mesh hash code.
                    string mesh_hash_code = _scene_objects[i].mesh_filter.sharedMesh.GetHashCode().ToString();

                    string mesh_path = "Assets/Meshes/Mesh" + mesh_hash_code + "/";

                    // Go through each LOD.
                    for (int j = 1; j < lod_levels; ++j) {
                        // Create new child game object.
                        GameObject obj = new GameObject(parent.name + "_lod" + j.ToString());
                        obj.transform.parent = parent.transform;
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localScale = Vector3.one;
                        obj.transform.localRotation = Quaternion.identity;

                        // Give child game object mesh filter and mesh renderer.
                        MeshFilter newMeshFilter = obj.AddComponent<MeshFilter>();
                        MeshRenderer newMeshRenderer = obj.AddComponent<MeshRenderer>();

                        string mesh_asset_path = mesh_path + mesh_hash_code + "_" + j.ToString() + ".asset";
                        string material_asset_path = "Assets/Meshes/DefaultMaterial.mat";

                        Mesh lod_mesh = (Mesh)AssetDatabase.LoadAssetAtPath(mesh_asset_path, typeof(Mesh));
                        Material lod_material = (Material)AssetDatabase.LoadAssetAtPath(material_asset_path, typeof(Material));

                        // Was the mesh successfully loaded?
                        if (lod_mesh) {
                            // Set mesh filter mesh to corresponding saved mesh.
                            newMeshFilter.mesh = lod_mesh;
                        } else {
                            Debug.Log("Coult not find mesh at: " + mesh_asset_path);
                        }

                        // Was the material successfully loaded?
                        if (lod_material) {
                            // Set mesh filter mesh to corresponding saved mesh.
                            newMeshRenderer.material = lod_material;
                        } else {
                            Debug.Log("Coult not find material at: " + material_asset_path);
                        }

                        group.dlods.Add(obj);
                    }
                }
            }
        }
    }
}
