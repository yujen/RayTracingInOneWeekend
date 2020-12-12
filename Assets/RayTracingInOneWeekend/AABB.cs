using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Axis-Aligned Bounding Boxe
/// </summary>
public class AABB
{
    public Vector3 min { get; private set; }
    public Vector3 max { get; private set; }




    public AABB(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }


    /// <summary>
    /// Optimized AABB hit method
    /// </summary>
    public bool IsHit(Ray r, float t_min, float t_max)
    {
        for (int a = 0; a < 3; a++)
        {
            float invD = 1f / r.direction[a];
            float t0 = (min[a] - r.origin[a]) * invD;
            float t1 = (max[a] - r.origin[a]) * invD;
            if (invD < 0f)
            {
                // swap
                var t = t0;
                t0 = t1;
                t1 = t;
            }

            t_min = t0 > t_min ? t0 : t_min;
            t_max = t1 < t_max ? t1 : t_max;

            if (t_max <= t_min)
            {
                return false;
            }
        }
        return true;

    }
    /*
    public bool IsHit(Ray r, float t_min, float t_max)
    {
        for (int a = 0; a < 3; a++)
        {
            var tmpMin = (min[a] - r.origin[a]) / r.direction[a];
            var tmpMax = (max[a] - r.origin[a]) / r.direction[a];

            var t0 = Mathf.Min(tmpMin, tmpMax);
            var t1 = Mathf.Max(tmpMin, tmpMax);

            t_min = Mathf.Max(t0, t_min);
            t_max = Mathf.Min(t1, t_max);

            if (t_max <= t_min)
            {
                return false;
            }

        }
        return true;

    }
    */
}
