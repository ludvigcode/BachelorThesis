using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPath : MonoBehaviour
{

    #region Public Variables
    public BezierSpline spline;
    public int frequency;
    public bool lookForward;

    //public Transform[] items;
    public Transform[] point_array;
    #endregion

    #region Public Functions
    public void update()
    {
        point_array = GetComponentsInChildren<Transform>();
    }

    public void clear()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void generate_points()
    {
        if (frequency <= 0)
        {
            return;
        }
        float stepSize = frequency;
        if (spline.loop || stepSize == 1)
        {
            stepSize = 1f / stepSize;
        }
        else
        {
            stepSize = 1f / (stepSize - 1);
        }

        for (int f = 0; f < frequency; f++)
        {
            Vector3 position = spline.get_point(f * stepSize);
            GameObject go = new GameObject("Point_" + f);
            go.transform.localPosition = position;
            if (lookForward)
            {
                go.transform.LookAt(position + spline.get_direction(f * stepSize));
            }
            go.transform.parent = transform;
        }
    }
    #endregion
}
