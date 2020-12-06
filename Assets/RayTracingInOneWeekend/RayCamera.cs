using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;





public class RayCamera : MonoBehaviour
{
    

    /// <summary>
    /// vertical field-of-view in degrees
    /// </summary>
    [SerializeField]
    public float verticalFov = 70f;

    [SerializeField]
    public float focalLength = 1f;

    [SerializeField]
    public Vector3 lookFrom = new Vector3(-2f, 2f, 1f);
    [SerializeField]
    public Vector3 lookAt = Vector3.zero;
    [SerializeField]
    public Vector3 vup = Vector3.up;


    [NonSerialized]
    public float viewportHeight;
    [NonSerialized]
    public float viewportWidth;
    [NonSerialized]
    public Vector3 horizontal;
    [NonSerialized]
    public Vector3 vertical;
    [NonSerialized]
    public Vector3 lowerLeftCorner;



    public void Setup(float aspectRatio)
    {
        float theta = verticalFov * Mathf.Deg2Rad;
        float h = Mathf.Tan(theta / 2f);
        viewportHeight = 2f * h;
        viewportWidth = aspectRatio * viewportHeight;


        Vector3 w = (lookFrom - lookAt).normalized;
        Vector3 u = Vector3.Cross(vup, w).normalized;
        Vector3 v = Vector3.Cross(w, u);


        horizontal = viewportWidth * u;
        vertical = viewportHeight * v;

        lowerLeftCorner = lookFrom - (horizontal / 2f) - (vertical / 2f) - w;
    }



    public Ray GetRay(float s, float t)
    {
        return new Ray(lookFrom, lowerLeftCorner + s * horizontal + t * vertical - lookFrom);
    }


    
}
