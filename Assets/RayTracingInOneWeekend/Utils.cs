using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Utils
{
    /// <summary>
    /// Returns a random real in [0, 1)
    /// </summary>
    static public float RandomNum()
    {
        return Random.Range(0f, 0.999999f);
    }

    /// <summary>
    /// Returns a random real in [min, max)
    /// </summary>
    static public float RandomNum(float min, float max)
    {
        return min + (max - min) * RandomNum();
    }


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

