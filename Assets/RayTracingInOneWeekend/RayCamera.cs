using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;





public class RayCamera
{
    [SerializeField]
    float viewportHeight = 2f;

    [SerializeField]
    float focalLength = 1f;

    [SerializeField]
    Vector3 origin = Vector3.zero;


    [ReadOnly]
    float viewportWidth;

    [ReadOnly]
    Vector3 horizontal;
    [ReadOnly]
    Vector3 vertical;
    [ReadOnly]
    Vector3 lowerLeftCorner;



    public RayCamera(int textureWidth, int textureHeight)
    {
        float aspectRatio = (float)textureWidth / (float)textureHeight;
        this.viewportWidth = aspectRatio * viewportHeight;


        this.horizontal = new Vector3(viewportWidth, 0f, 0f);
        this.vertical = new Vector3(0f, viewportHeight, 0f);

        lowerLeftCorner = origin - (horizontal / 2f) - (vertical / 2f) - new Vector3(0f, 0f, focalLength);
    }



    public Ray GetRay(float u, float v)
    {
        return new Ray(origin, lowerLeftCorner + u * horizontal + v * vertical - origin);
    }


    
}
