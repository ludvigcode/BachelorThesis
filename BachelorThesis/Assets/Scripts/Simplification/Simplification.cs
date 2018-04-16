// SimplificationTools.cs
// Fredrik Linde TA15
// Mesh Incremental Decimation Simplification Utility
// Using the UnityMeshSimplifier by Whinarn
// All materials belong to its rightful owner.
// https://github.com/Whinarn/UnityMeshSimplifier

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

        //set_lod(lod_array.Count - 1);
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

public class Simplification : MonoBehaviour
{

    #region Public Variables
    public int lod_levels;
    public Dictionary<int, List<Mesh>> table;
    #endregion

    #region Private Variables
    private List<RenderingTarget> _scene_objects;
    private int _counter = 0;
    #endregion

    #region Public Variables

    public void create_lod()
    {
        // Create the dictionary.
        table = new Dictionary<int, List<Mesh>>();

        // Create the a list for rendering targets in scene.
        _scene_objects = new List<RenderingTarget>();

        // Get all mesh filters in the scene.
        MeshFilter[] filters = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

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
                Mesh meshCopy = Instantiate(filter.sharedMesh) as Mesh;

                _generate_lod_versions(meshCopy, array, lod_levels);

                // Store LOD array as value in the dictionary given the original mesh hash code as key.
                table.Add(key, array);

                // Add new rendering target to the scene with LOD version list from dictionary.
                _scene_objects.Add(new RenderingTarget(filter, array));
                
            }

        }
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
            simplifier.SimplifyMesh(0.5f);

            // Get the resulting mesh. 
            Mesh destMesh = simplifier.ToMesh();

            // Set the resulting mesh to the current LOD version.
            list.Add(destMesh);

        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        // Remove this later, just for the sake of testing.

        if (Input.GetKeyDown(KeyCode.R))
        {
            _counter++;
            _counter = Mathf.Clamp(_counter, 0, lod_levels - 1);

            foreach (RenderingTarget rt in _scene_objects)
            {
                rt.set_lod(_counter);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _counter--;
            _counter = Mathf.Clamp(_counter, 0, lod_levels - 1);

            foreach (RenderingTarget rt in _scene_objects)
            {
                rt.set_lod(_counter);
            }
        }
    }

    #endregion


}