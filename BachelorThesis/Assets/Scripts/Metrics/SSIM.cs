using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

public static class SSIM {
    #region Public Methods
    // To be used for comparing stored images.
    public static float compute_mssim_string(string imgpath1, string imgpath2) {
        // Adds the application data path which leads to the project's asset folder.
        string path1 = Application.dataPath + "/" + imgpath1;
        string path2 = Application.dataPath + "/" + imgpath2;

        // Does any of the images exist?
        if (File.Exists(path1) && File.Exists(path2)) {
            return OpenCVInterop.mssim_string(path1, path2);
        } else {
            Debug.Log("Could not find the following image paths:\n" + path1 + "\n" + path2);
            return -1.0f;
        }
    }

    public static float compute_mssim_textures(Texture2D reference, Texture2D image, int width, int height) {
        byte[] ref_bytes = reference.EncodeToPNG();
        byte[] img_bytes = image.EncodeToPNG();
        return compute_mssim_byte(ref_bytes, img_bytes, width, height);
    }

    // To be used when comparing screenshots in run-time.
    public static float compute_mssim_byte(byte[] reference, byte[] image, int width, int height) {
        // Allocate unmanaged memory.
        IntPtr ua1 = Marshal.AllocHGlobal(reference.Length);
        IntPtr ua2 = Marshal.AllocHGlobal(image.Length);

        // Copy content from byte arrays into the unmanaged memory.
        Marshal.Copy(reference, 0, ua1, reference.Length);
        Marshal.Copy(image, 0, ua2, image.Length);

        float result = OpenCVInterop.mssim_byte(ua1, ua2, width, height);

        // De-allocate the unmanaged memory.
        Marshal.FreeHGlobal(ua1);
        Marshal.FreeHGlobal(ua2);

        return result;
    }
    #endregion
}

// Define the functions which can be called from the .dll.
internal static class OpenCVInterop {
    [DllImport("SSIM")]
    internal static extern float mssim_string(string img1, string img2);
    [DllImport("SSIM")]
    internal static extern float mssim_byte(IntPtr img1, IntPtr img2, int w, int h);
}
