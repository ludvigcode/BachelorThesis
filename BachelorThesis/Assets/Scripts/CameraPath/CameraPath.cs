using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraPath : MonoBehaviour {

    public SceneGrid grid;
    public BezierSpline spline;
    public int frequency;
    public bool lookForward;
    public float handle_size = 1.0f;
    public float steptime;
    public bool save_images;
    public int img_width = 800;
    public int img_height = 600;
    public string folder_path = null;
    public string active_folder = null;
    public List<GameObject> point_array = null;

    private enum State {
        BEGIN,
        REF,
        OURS,
        UNITY,
        DONE
    }

    private State _active_state = State.BEGIN;

    private Camera _m_cam;
    private float _progress = 0.0f;
    private int _index = 0;
    private bool _generated;
    private bool _finished;

    public void clear() {

        foreach (GameObject go in point_array) {
            DestroyImmediate(go);
        }
        point_array.Clear();
        _generated = false;
    }

    public void generate_points() {

        if (_generated == false) {

            if (frequency <= 0) {
                return;
            }

            if (!spline) {
                Debug.Log("No spline attached to camera path!");
                return;
            }

            float stepSize = frequency;
            if (spline.loop || stepSize == 1) {
                stepSize = 1f / stepSize;
            } else {
                stepSize = 1f / (stepSize - 1);
            }

            point_array = new List<GameObject>();

            for (int f = 0; f < frequency; ++f) {
                Vector3 position = spline.get_point(f * stepSize);
                GameObject go = new GameObject("Point_" + f);
                go.transform.localPosition = position;
                if (lookForward) {
                    go.transform.LookAt(position + spline.get_direction(f * stepSize));
                }
                go.transform.parent = transform;
                point_array.Add(go);
            }
            _generated = true;
        } else {
            Debug.Log("Clear existing points before generate new points!");
        }
    }

    private void Start() {
        _m_cam = Camera.main;
        _finished = true;
    }

    private void Update() {
        if (_active_state == State.DONE) {
            return;
        }

        if (_finished) {
            ++_active_state;
            _finished = false;

            if (_active_state == State.REF) {
                active_folder = folder_path + "/ref";
                Directory.CreateDirectory(active_folder);
                DLODGroup[] dlods = FindObjectsOfType<DLODGroup>();
                foreach (DLODGroup d in dlods) {
                    d.set_to_original();
                }
            } else if (_active_state == State.OURS) {
                active_folder = folder_path + "/ours";
                Directory.CreateDirectory(active_folder);
            } else if (_active_state == State.UNITY) {
                active_folder = folder_path + "/unity";
                Directory.CreateDirectory(active_folder);
                DLODGroup[] dlods = FindObjectsOfType<DLODGroup>();
                foreach (DLODGroup d in dlods) {
                    d.set_to_original();
                    GameObject go = d.gameObject;
                    LODGroup lod_grp = go.AddComponent<LODGroup>();
                    LOD[] lods = new LOD[5];

                    MeshRenderer[] m1 = new MeshRenderer[1];
                    d.dlods[2].SetActive(true);
                    m1[0] = d.dlods[0].GetComponent<MeshRenderer>();
                    lods[0].renderers = m1;
                    lods[0].screenRelativeTransitionHeight = 0.5f;

                    MeshRenderer[] m2 = new MeshRenderer[1];
                    d.dlods[2].SetActive(true);
                    m2[0] = d.dlods[1].GetComponent<MeshRenderer>();
                    lods[1].renderers = m1;
                    lods[1].screenRelativeTransitionHeight = 0.25f;

                    MeshRenderer[] m3 = new MeshRenderer[1];
                    d.dlods[2].SetActive(true);
                    m1[0] = d.dlods[2].GetComponent<MeshRenderer>();
                    lods[2].renderers = m1;
                    lods[2].screenRelativeTransitionHeight = 0.125f;

                    MeshRenderer[] m4 = new MeshRenderer[1];
                    d.dlods[2].SetActive(true);
                    m1[0] = d.dlods[3].GetComponent<MeshRenderer>();
                    lods[3].renderers = m1;
                    lods[3].screenRelativeTransitionHeight = 0.075f;

                    MeshRenderer[] m5 = new MeshRenderer[1];
                    d.dlods[2].SetActive(true);
                    m1[0] = d.dlods[4].GetComponent<MeshRenderer>();
                    lods[4].renderers = m1;
                    lods[4].screenRelativeTransitionHeight = 0.01f;

                    lod_grp.SetLODs(lods);
                }

            } else if (_active_state == State.DONE) {
                return;
            }
        }



        _traverse_path();
    }

    private void OnDrawGizmos() {
        if (point_array.Count <= 2) {
            return;
        }

        Gizmos.color = Color.red;

        foreach (GameObject go in point_array) {
            Gizmos.DrawSphere(go.transform.position, 1.0f);
        }

        Gizmos.color = Color.magenta;

        for (int i = 0; i < point_array.Count - 1; ++i) {
            Gizmos.DrawLine(point_array[i].transform.position, point_array[i + 1].transform.position);
        }

        Gizmos.DrawLine(point_array[point_array.Count - 1].transform.position, point_array[0].transform.position);
    }

    private void _traverse_path() {

        _m_cam.transform.position = point_array[_index].transform.position;
        _m_cam.transform.rotation = point_array[_index].transform.rotation;

        if (_active_state == State.OURS) {
            grid.set_dlods(_m_cam);
        }

        if (save_images) {
            Texture2D tex = _take_screen_shoot(img_width, img_height);
            File.WriteAllBytes(active_folder + "/" + _index.ToString() + ".png", tex.EncodeToPNG());
            Destroy(tex);
        }

        ++_index;

        if (_index >= point_array.Count) {
            _finished = true;
            _index = 0;
        }
    }

    private Texture2D _take_screen_shoot(int width, int height) {
        RenderTexture tex = new RenderTexture(width, height, 16);
        Texture2D screen_shot = new Texture2D(width, height, TextureFormat.RGB24, false);

        _m_cam.targetTexture = tex;
        _m_cam.Render();

        RenderTexture.active = tex;
        screen_shot.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        RenderTexture.active = null;
        _m_cam.targetTexture = null;
        Destroy(tex);

        return screen_shot;
    }
}
