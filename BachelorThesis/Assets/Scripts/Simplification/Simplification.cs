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

public class RenderingTarget
{
    //public GameObject game_object;
    public MeshFilter mesh_filter;
    public List<Mesh> lod_array;

    public RenderingTarget(MeshFilter filter, List<Mesh> mesh_list)
    {
        // Cache the mesh filter and create mesh array.
        mesh_filter = filter;
        lod_array = new List<Mesh>(mesh_list);
        
        // Set to highest quality.
        //set_lod(0);
    }

    // Set the rendering target LOD version.
    public void set_lod(int index)
    {
        set_mesh(lod_array[index]);
    }

    // Set the rendering target mesh.
    public void set_mesh(Mesh mesh)
    {
        mesh_filter.mesh = mesh;
    }
}

public class Simplification
{

    #region Public Variables
    public int lod_levels;
    public float quality;
    public Dictionary<int, List<Mesh>> table;
    #endregion

    #region Private Variables
    private List<RenderingTarget> _scene_objects;
    #endregion

    #region Public Variables

    public void create_lod()
    {
        // Create the dictionary.
        table = new Dictionary<int, List<Mesh>>();

        // Create the a list for rendering targets in scene.
        _scene_objects = new List<RenderingTarget>();

        // Get all mesh filters in the scene.
        MeshFilter[] filters = GameObject.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

        // Loop through each mesh filter and create rendering targets from their meshes.
        foreach (MeshFilter filter in filters)
        {
            // Get the hash code of the shared mesh, the original mesh. This will remain identical throughout the applicaton.
            // filter.mesh returns an instantiated clone of the mesh, which will always produce a unique hash code.
            int key = filter.sharedMesh.GetHashCode();

            // Has this mesh already been processed? 
            if (table.ContainsKey(key))
            {
                Debug.Log("Mesh with key " + key.ToString() + " already exists. Using existing LOD meshes...");

                // Since the dictionary already holds the key to this mesh, the LOD versions have 
                // already been generated. Take the references from the mesh list in the dictionary
                // and give them to the new rendering target.
                _scene_objects.Add(new RenderingTarget(filter, table[key]));

            }
            else
            {
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

        // Save the LODs to asset files.
        _save_lods_to_file();

        // Create gameobjects from the saved asset files.
        _create_dlod_objects();
    }
    #endregion

    #region Private Functions

    private void _generate_lod_versions(Mesh mesh, List<Mesh> list, int lod_count)
    {
        // The first instance in the LOD array is the default mesh.
        list.Add(mesh);

        // Initialize the mesh simplifier.
        MeshSimplifier simplifier = new MeshSimplifier();

        // Iterate through each LOD version.
        for (int i = 1; i < lod_count; i++)
        {
            // Initialize the mesh simplifier with the previous LOD version mesh.
            simplifier.Initialize(list[i - 1]);

            // Simplify by a given percentage (0.5 = 50% by default).
            simplifier.SimplifyMesh(quality);

            // Get the resulting mesh. 
            Mesh destMesh = simplifier.ToMesh();

            // Set the resulting mesh to the current LOD version.
            list.Add(destMesh);

        }
    }

    private void _save_lods_to_file()
    {
        foreach (KeyValuePair<int, List<Mesh>> hash in table)
        {
            string directorypath = "Assets/Meshes/Mesh" + hash.Key.ToString();
            try
            {
                if (!Directory.Exists(directorypath))
                {
                    Directory.CreateDirectory(directorypath);
                }

            }
            catch (IOException ex)
            {
                Debug.Log(ex.Message);
            }

            int lod = 0;
            foreach (Mesh mesh in hash.Value)
            {
                if (lod != 0)
                {
                    string savepath = directorypath + "/" + hash.Key.ToString() + "_" + lod.ToString() + ".asset";
                    save_mesh_obj(mesh, savepath);
                    // WaitForFile(savepath);
                }
                lod++;
            }
        }
    }

    private void WaitForFile(string file)
    {
        float t = 1.0f;
        // Wait for file to become valid or 5 seconds max
        while (!System.IO.File.Exists(file)) {

            t -= Time.deltaTime;

            if(t == 0)
            {
                t = 1.0f;
                Debug.Log(file + " not written yet...");
            }
        }

        Debug.Log(file + " has been written!");
    }

    private void save_mesh_obj(Mesh mesh, string path)
    {
        AssetDatabase.CreateAsset(mesh, path);
    }

    private void _create_dlod_objects()
    {
        // Go through each scene object (mesh filter).
        for (int i = 0; i < _scene_objects.Count; i++)
        {
            // Get parent game object to LOD0. 
            GameObject parent = _scene_objects[i].mesh_filter.gameObject.transform.parent.gameObject;

            // Get the shared mesh hash code.
            string mesh_code = _scene_objects[i].mesh_filter.sharedMesh.GetHashCode().ToString();

            string directorypath = "Assets/Meshes/Mesh" + mesh_code + "/";

            // Go through each LOD.
            for (int j = 1; j < lod_levels; j++)
            {
                // Create new child game object.
                GameObject obj = new GameObject(parent.name + "_lod" + j.ToString());
                obj.transform.parent = parent.transform;
                obj.transform.localPosition = Vector3.zero;

                // Give child game object mesh filter and mesh renderer.
                MeshFilter newMeshFIlter = obj.AddComponent<MeshFilter>();
                MeshRenderer newMeshRenderer = obj.AddComponent<MeshRenderer>();

                string asset_path = directorypath + mesh_code + "_" + j.ToString() + ".asset";
                Mesh lod_mesh = (Mesh)AssetDatabase.LoadAssetAtPath(asset_path, typeof(Mesh));

                // Was the mesh successfully loaded?
                if (lod_mesh)
                {
                    // Set mesh filter mesh to corresponding saved mesh.
                    newMeshFIlter.mesh = lod_mesh;
                }
                else
                {
                    Debug.Log("Coult not find mesh at: " + asset_path);
                }
            }

        }
    }
    #endregion

}