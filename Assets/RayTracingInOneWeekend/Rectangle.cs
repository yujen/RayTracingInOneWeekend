using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Rectangle : Hittable
{
    protected float x0, x1, y0, y1, k;
    protected ObjectMaterial objMaterial;


    public Rectangle(float x0, float x1, float y0, float y1, float k, ObjectMaterial objMaterial)
    {
        this.x0 = x0;
        this.x1 = x1;
        this.y0 = y0;
        this.y1 = y1;
        this.k = k;
        this.objMaterial = objMaterial;
    }



    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        // The bounding box must have non-zero width in each dimension, so pad the Z dimension a small amount.
        output = new AABB(new Vector3(x0, y0, k - 0.0001f), new Vector3(x1, y1, k + 0.0001f));

        return true;
    }

    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        float t = (k - ray.origin.z) / ray.direction.z;
        if (t < t_min || t > t_max)
        {
            return false;
        }
            
        float x = ray.origin.x + t * ray.direction.x;
        float y = ray.origin.y + t * ray.direction.y;
        if (x < x0 || x > x1 || y < y0 || y > y1)
        {
            return false;
        }
            

        hitRecord.uv = new Vector2((x - x0) / (x1 - x0), (y - y0) / (y1 - y0));
        hitRecord.t = t;
        hitRecord.SetFaceNormal(ray, new Vector3(0f, 0f, 1f));  // outwardNormal
        hitRecord.objMaterial = objMaterial;
        hitRecord.p = ray.At(t);


        return true;
    }
}

