using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine;

public class SSIM : MonoBehaviour
{

    #region Public Variables

    #endregion

    #region Private Variables

    #endregion

    #region Public Functions
    // To be used for comparing stored images.
    public static float compute_mssim_string(string imgpath1, string imgpath2)
    {
        // Adds the application data path which leads to the project's asset folder.
        string path1 = Application.dataPath + "/" + imgpath1;
        string path2 = Application.dataPath + "/" + imgpath2;

        // Does any of the images exist?
        if (File.Exists(path1) && File.Exists(path2))
        {
            return OpenCVInterop.mssim_string(path1, path2);
        }
        else
        {
            Debug.Log("Could not find the following image paths:\n" + path1 + "\n" + path2);
            return -1.0f;
        }
    }

    // To be used when comparing screenshots in run-time.
    public static float compute_mssim_byte(byte[] imgarray1, byte[] imgarray2, int w, int h)
    {
        // Allocate unmanaged memory.
        IntPtr ua1 = Marshal.AllocHGlobal(Marshal.SizeOf(imgarray1[0]) * imgarray1.Length);
        IntPtr ua2 = Marshal.AllocHGlobal(Marshal.SizeOf(imgarray2[0]) * imgarray2.Length);

        // Copy content from byte arrays into the unmanaged memory.
        Marshal.Copy(imgarray1, 0, ua1, imgarray1.Length);
        Marshal.Copy(imgarray2, 0, ua2, imgarray2.Length);

        float result = OpenCVInterop.mssim_byte(ua1, ua2, w, h);

        // De-allocate the unmanaged memory.
        Marshal.FreeHGlobal(ua1);
        Marshal.FreeHGlobal(ua2);

        return result;
    }
    #endregion

    #region Private Functions

    #endregion

    // Use this for initialization
    void Start()
    {
        //float index = compute_mssim_string("Images/BoatOriginal.jpg", "Images/BoatNoise.jpg");
        //Debug.Log("MSSIM: " + index);
    }
}

// Define the functions which can be called from the .dll.
internal static class OpenCVInterop
{
    [DllImport("SSIM")]
    internal static extern float mssim_string(string img1, string img2);
    [DllImport("SSIM")]
    internal static extern float mssim_byte(IntPtr img1, IntPtr img2, int w, int h);
}
