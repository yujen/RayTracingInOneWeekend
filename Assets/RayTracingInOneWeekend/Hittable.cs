using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class Ray
{
    public Ray(Vector3 origin, Vector3 direction)
    {
        this.origin = origin;
        this.direction = direction;
    }

    /// <summary>
    /// 射線頂端位置
    /// </summary>
    public Vector3 At(float t)
    {
        return origin + direction * t;
    }


    public Vector3 origin;
    public Vector3 direction;
}

public class HitRecord
{
    public Vector3 p;
    public Vector3 normal;
    public float t;
}

public abstract class Hittable
{
    abstract public bool IsHit(Ray ray, float t_min, float t_max, HitRecord hitRecord);

}


