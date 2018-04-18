using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
    #region Public Variables
    [SerializeField]
    public Vector3[] points;
    #endregion

    #region Private Variables
    [SerializeField]
    private bool _loop;
    private bool initialized = false;

    private BezierControlPointMode[] _modes;
    #endregion

    #region Public Functions
    public void init()
    {
        points = new Vector3[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
        _modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };

        initialized = true;
    }

    public bool is_initialized()
    {
        return initialized;
    }

    public bool loop
    {
        get
        {
            return _loop;
        }
        set
        {
            _loop = value;
            if (value == true)
            {
                _modes[_modes.Length - 1] = _modes[0];
                set_control_point(0, points[0]);
            }
        }
    }

    public BezierControlPointMode get_control_point_mode(int index)
    {
        return _modes[(index + 1) / 3];
    }

    public void set_control_point_mode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        _modes[modeIndex] = mode;
        if (_loop)
        {
            if (modeIndex == 0)
            {
                _modes[_modes.Length - 1] = mode;
            }
            else if (modeIndex == _modes.Length - 1)
            {
                _modes[0] = mode;
            }
        }
        _enforce_mode(index);
    }

    public int control_point_count
    {
        get
        {
            return points.Length;
        }
    }

    public Vector3 get_control_point(int index)
    {
        return points[index];
    }

    public void set_control_point(int index, Vector3 point)
    {
        if (index % 3 == 0)
        {
            Vector3 delta = point - points[index];
            if (_loop)
            {
                if(index == 0)
                {
                    points[1] += delta;
                    points[points.Length - 2] += delta;
                    points[points.Length - 1] = point;
                }
                else if(index == points.Length - 1)
                {
                    points[0] = point;
                    points[1] += delta;
                    points[index - 1] += delta;
                }
                else
                {
                    points[index - 1] += delta;
                    points[index + 1] += delta;
                }
            }
            else
            {
                if (index > 0)
                {
                    points[index - 1] += delta;
                }
                if (index + 1 < points.Length)
                {
                    points[index + 1] += delta;
                }
            }
        }
        points[index] = point;
        _enforce_mode(index);
    }

    public Vector3 get_point(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * curve_count;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.get_point_cubic(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    public Vector3 get_velocity(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * curve_count;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(
            Bezier.get_first_derative_cubic(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 get_direction(float t)
    {
        return get_velocity(t).normalized;
    }

    public int curve_count
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public void add_curve()
    {
        if (!initialized)
        {
            Debug.Log("Spline must be initialized before adding curves!");
            return;
        }
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1.0f;
        points[points.Length - 3] = point;
        point.x += 1.0f;
        points[points.Length - 2] = point;
        point.x += 1.0f;
        points[points.Length - 1] = point;

        Array.Resize(ref _modes, _modes.Length + 1);
        _modes[_modes.Length - 1] = _modes[_modes.Length - 2];
        _enforce_mode(points.Length - 4);

        if (_loop)
        {
            points[points.Length - 1] = points[0];
            _modes[_modes.Length - 1] = _modes[0];
            _enforce_mode(0);
        }
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
        _modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };
    }

    #endregion

    #region Private Functions
    private void _enforce_mode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = _modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !_loop && (modeIndex == 0 || modeIndex == _modes.Length - 1))
        {
            return;
        }

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if (index <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if(fixedIndex < 0)
            {
                fixedIndex = points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if(enforcedIndex >= points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if(fixedIndex >= points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if(enforcedIndex < 0)
            {
                enforcedIndex = points.Length - 2;
            }
        }
        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        }
        points[enforcedIndex] = middle + enforcedTangent;
    }
    #endregion
}