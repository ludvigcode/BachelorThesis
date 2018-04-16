using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Simplification))]
public class SimplificationEditor : Editor {

    public override void OnInspectorGUI()
    {
        Simplification simp = (Simplification)target;

        simp.lod_levels = EditorGUILayout.IntField(simp.lod_levels);

        if (GUILayout.Button("Create LODs"))
        {
            simp.create_lod();

            foreach (var hash in simp.table)
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
                foreach (Mesh mesh in simp.table[hash.Key])
                {
                    string savepath = directorypath + "/" + hash.Key.ToString() + "_" + lod.ToString() + ".asset";
                    save_mesh_obj(mesh, savepath);
                    lod++;
                }
            }
        }
    }

    private void save_mesh_obj(Mesh mesh, string path)
    {
        AssetDatabase.CreateAsset(mesh, path);
    }
}
