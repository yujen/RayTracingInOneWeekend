﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Returns a random real in [0, 1)
    /// </summary>
    public static float RandomNum
    {
        get
        {
            return Random.Range(0f, 0.999999f);
        }
    }

    /// <summary>
    /// Returns a random real in [min, max)
    /// </summary>
    public static float RandomRange(float min, float max)
    {
        return min + (max - min) * RandomNum;
    }


    /// <summary>
    /// Uniform Sampling a Hemisphere
    /// </summary>
    public static Vector3 RandomCosineDirection
    {
        get
        {
            float r1 = RandomNum;
            float r2 = RandomNum;
            float z = Mathf.Sqrt(1f - r2);

            float phi = 2f * Mathf.PI * r1;
            float x = Mathf.Cos(phi) * Mathf.Sqrt(r2);
            float y = Mathf.Sin(phi) * Mathf.Sqrt(r2);

            return new Vector3(x, y, z);
        }
    }

    public static Vector3 RandomSphere(float radius, float distance_squared)
    {
        float r1 = RandomNum;
        float r2 = RandomNum;
        float z = 1f + r2 * (Mathf.Sqrt(1f - radius * radius / distance_squared) - 1f);

        float phi = 2f * Mathf.PI * r1;
        float x = Mathf.Cos(phi) * Mathf.Sqrt(1f - z * z);
        float y = Mathf.Sin(phi) * Mathf.Sqrt(1f - z * z);

        return new Vector3(x, y, z);
    }


    public static Vector3 RandomInHemisphere(this Vector3 normal)
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
    public static bool IsNearZero(this Vector3 v)
    {
        float s = 0.00001f;
        return (Mathf.Abs(v.x) < s) && (Mathf.Abs(v.y) < s) && (Mathf.Abs(v.z) < s);
    }






}

