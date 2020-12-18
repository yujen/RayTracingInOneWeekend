using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Utils
{


    static public Vector3 RandomInHemisphere(this Vector3 normal)
    {
        var insideUnitSphere = Random.insideUnitSphere;
        if (Vector3.Dot(insideUnitSphere, normal) > 0f)
        {
            return insideUnitSphere;
        }
        else
        {
            return -insideUnitSphere;
        }
    }


    /// <summary>
    /// Return true if the vector is close to zero in all dimensions.
    /// </summary>
    static public bool IsNearZero(this Vector3 v)
    {
        float s = 0.0001f;
        return (Mathf.Abs(v.x) < s) && (Mathf.Abs(v.y) < s) && (Mathf.Abs(v.z) < s);
    }

}

