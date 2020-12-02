using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Sphere : Hittable
{
    public Vector3 center;
    public float radius;


    public Sphere(Vector3 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }


    public override bool IsHit(Ray ray, float t_min, float t_max, HitRecord hitRecord)
    {
        Vector3 oc = ray.origin - center;
        float a = ray.direction.sqrMagnitude;
        float half_b = Vector3.Dot(oc, ray.direction);
        float c = oc.sqrMagnitude - radius * radius;

        float discriminant = half_b * half_b - a * c;

        if (discriminant < 0)
        {
            return false;
        }

        float sqrtd = Mathf.Sqrt(discriminant);

        // Find the nearest root that lies in the acceptable range.
        float root = (-half_b - sqrtd) / a;
        if (root < t_min || t_max < root)
        {
            root = (-half_b + sqrtd) / a;
            if (root < t_min || t_max < root)
            {
                return false;
            }
        }

        hitRecord.t = root;
        hitRecord.p = ray.At(hitRecord.t);
        hitRecord.normal = (hitRecord.p - center) / radius;

        return true;
    }
}
