using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraPath : MonoBehaviour {

    #region Public Variables
    public BezierSpline spline;
    public int frequency;
    public bool lookForward;
    public bool loop;
    public float handle_size = 1.0f;
    public float steptime;
    public bool save_images;
    public int ímg_width = 800;
    public int img_height = 600;
    public string folder_path = null;
    public List<GameObject> point_array = null;
    #endregion

    #region Private Variables
    private Camera _m_cam;
    private float _progress = 0.0f;
    private int _index = 0;
    private bool _generated;
    private bool _finished;
    #endregion

    #region Public Functions

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

            for (int f = 0; f < frequency; f++) {
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
    #endregion

    #region Private Functions
    private void Start() {
        _m_cam = Camera.main;
        _finished = false;
    }

    private void Update() {

        if (_finished != true) {
            _traverse_path();
        }
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

        _progress += Time.deltaTime;

        if (_progress > steptime) {
            _progress = 0.0f;

            _m_cam.transform.position = point_array[_index].transform.position;
            _m_cam.transform.rotation = point_array[_index].transform.rotation;

            if (save_images) {
                Texture2D tex = _take_screen_shoot(ímg_width, img_height);
                File.WriteAllBytes(folder_path + "/" + _index.ToString() + ".png", tex.EncodeToPNG());
                Destroy(tex);
            }

            _index++;

            if (_index >= point_array.Count) {
                if (loop) {
                    _index = 0;
                } else {
                    _finished = true;
                    Debug.Log("Scene has been traversed!");
                }
            }
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
    #endregion
}
