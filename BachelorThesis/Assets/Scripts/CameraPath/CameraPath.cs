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
    public void create_decoration()
    {
        //if (frequency <= 0 || items == null || items.Length == 0)
        //{
        //    return;
        //}
        //float stepSize = frequency * items.Length;
        //if (spline.loop || stepSize == 1)
        //{
        //    stepSize = 1f / stepSize;
        //}
        //else
        //{
        //    stepSize = 1f / (stepSize - 1);
        //}

        //for (int p = 0, f = 0; f < frequency; f++)
        //{
        //    for (int i = 0; i < items.Length; i++, p++)
        //    {
        //        Transform item = Instantiate(items[i]) as Transform;
        //        Vector3 position = spline.get_point(p * stepSize);
        //        item.transform.localPosition = position;
        //        if (lookForward)
        //        {
        //            item.transform.LookAt(position + spline.get_direction(p * stepSize));
        //        }
        //        item.transform.parent = transform;
        //    }
        //}
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

        for (int p = 0, f = 0; f < frequency; f++)
        {
            Vector3 position = spline.get_point(p * stepSize);

            GameObject go = new GameObject("Point");
            go.transform.localPosition = position;
            if (lookForward)
            {
                go.transform.LookAt(position + spline.get_direction(p * stepSize));
            }
            go.transform.parent = transform;
            p++;
        }
    }
    #endregion

    #region Private Functions

    private void Awake()
    {

    }
    #endregion
}
