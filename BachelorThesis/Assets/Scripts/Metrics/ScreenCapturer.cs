using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenCapturer : MonoBehaviour {

    #region Public Variables
    public int capture_width;
    public int capture_height;

    // Optimize for many screenshots. No objects will be destroyed so future screenshots are fast.
    public bool optimize;
    #endregion

    #region Private Variables
    private Rect _rect;
    private RenderTexture _render_texture;
    private Texture2D _screenshot;

    private Camera _main_camera;
    #endregion

    #region Public Functions

    public byte[] capture_byte_screenshot()
    {
        // Create screenshot objects if necessary.
        if (_render_texture == null)
        {
            // creates off-screen render texture that can rendered into.
            _rect = new Rect(0, 0, capture_width, capture_height);
            _render_texture = new RenderTexture(capture_width, capture_height, 24);
            _screenshot = new Texture2D(capture_width, capture_height, TextureFormat.RGB24, false);
        }

        // Set the camera render target texture.
        _main_camera.targetTexture = _render_texture;
        _main_camera.Render();

        // read pixels will read from the currently active render texture so make our offscreen 
        // render texture active and then read the pixels.
        RenderTexture.active = _render_texture;
        _screenshot.ReadPixels(_rect, 0, 0);

        // reset active camera texture and render texture.
        _main_camera.targetTexture = null;
        RenderTexture.active = null;

        byte[] fileData = null;
        fileData = _screenshot.GetRawTextureData();

        // cleanup if needed.
        if (optimize == false)
        {
            Destroy(_render_texture);
            _render_texture = null;
            _screenshot = null;
        }

        return fileData;
    }
    #endregion

    #region Private Functions

    private void average_ssim_test() {

        // Force environment path to plugin folder.

        //String currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
        //String dllPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Plugins";
        //if (currentPath.Contains(dllPath) == false) {
        //    Environment.SetEnvironmentVariable("PATH", currentPath + Path.PathSeparator + dllPath, EnvironmentVariableTarget.Process);
        //}

        _main_camera = Camera.main;
        byte[] image1 = capture_byte_screenshot();
        byte[] image2 = capture_byte_screenshot();
        float start_time = 0;

        float[] numbers = new float[10];

        for (int i = 0; i < 10; i++) {
            start_time = Time.realtimeSinceStartup;
            float index = SSIM.compute_mssim_byte(image1, image2, capture_width, capture_height);
            numbers[i] = Time.realtimeSinceStartup - start_time;
            //Debug.Log("MSSIM Execution Time: " + (Time.realtimeSinceStartup - start_time));
            //Debug.Log("MSSIM: " + index);
        }

        Debug.Log("MSSIM Average Execution Time: " + (numbers.Average()));
    }
    #endregion
    
}
