using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Sphere : Hittable
{
    public Vector3 center;
    public float radius;
    public ObjectMaterial objMaterial;


    public Sphere(Vector3 center, float radius, ObjectMaterial objMaterial)
    {
        this.center = center;
        this.radius = radius;
        this.objMaterial = objMaterial;
    }


    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
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
        Vector3 outwardNormal = (hitRecord.p - center) / radius;
        hitRecord.SetFaceNormal(ray, outwardNormal);
        hitRecord.objMaterial = objMaterial;
        hitRecord.uv = GetUV(outwardNormal);

        return true;
    }

    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        var pos = new Vector3(radius, radius, radius);
        output = new AABB(center - pos, center + pos);

        return true;
    }


    /// <summary>
    /// Texture coordinates for sphere
    /// </summary>
    /// <param name="p">A given point on the sphere of radius one, centered at the origin.</param>
    public Vector2 GetUV(Vector3 p)
    {
        float theta = Mathf.Acos(-p.y);
        float phi = Mathf.Atan2(-p.z, p.x) + Mathf.PI;
        float u = phi / (2f * Mathf.PI);
        float v = theta / Mathf.PI;

        return new Vector2(u, v);
    }

}


class MovingSphere : Hittable
{
    public Vector3 center0, center1;
    public float time0, time1;
    public float radius;
    public ObjectMaterial objMaterial;


    public MovingSphere(Vector3 center0, Vector3 center1, float time0, float time1, float radius, ObjectMaterial objMaterial)
    {
        this.center0 = center0;
        this.center1 = center1;
        this.time0 = time0;
        this.time1 = time1;
        this.radius = radius;
        this.objMaterial = objMaterial;

    }


    public Vector3 GetCenter(float time)
    {
        return center0 + ((time - time0) / (time1 - time0)) * (center1 - center0);
    }



    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        Vector3 oc = ray.origin - GetCenter(ray.time);
        float a = ray.direction.sqrMagnitude;
        float half_b = Vector3.Dot(oc, ray.direction);
        float c = oc.sqrMagnitude - radius * radius;
        float discriminant = half_b * half_b - a * c;

        if (discriminant < 0f)
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
        Vector3 outwardNormal = (hitRecord.p - GetCenter(ray.time)) / radius;
        hitRecord.SetFaceNormal(ray, outwardNormal);
        hitRecord.objMaterial = objMaterial;

        return true;
    }

    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        var pos = new Vector3(radius, radius, radius);

        var center0 = GetCenter(time0);
        var aabb0 = new AABB(center0 - pos, center0 + pos);
        var center1 = GetCenter(time1);
        var aabb1 = new AABB(center1 - pos, center1 + pos);

        output = SurroundingBox(aabb0, aabb1);

        return true;
    }

}

