using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
    #region Public Variables
    public Vector3[] points;
    #endregion

    #region Public Functions
    public Vector3 GetPoint(float t)
    {
        return transform.TransformPoint(Bezier.GetPointCubic(points[0], points[1], points[2], points[3], t));
    }

    public Vector3 GetVelocity(float t)
    {
        return transform.TransformPoint(
            Bezier.GetFirstDerativeCubic(points[0], points[1], points[2], points[3], t)) - transform.position;
    }

    public Vector2 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1.0f;
        points[points.Length - 3] = point;
        point.x += 1.0f;
        points[points.Length - 2] = point;
        point.x += 1.0f;
        points[points.Length - 1] = point;
    }

    public void Reset()
    {
        points = new Vector3[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
    }

    #endregion

    #region Private Functions

    #endregion
}