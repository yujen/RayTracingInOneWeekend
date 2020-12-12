using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class Hittable
{
    abstract public bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord);


    abstract public bool BoundingBox(float time0, float time1, out AABB output);


    protected AABB SurroundingBox(AABB box0, AABB box1)
    {
        var min0 = box0.min;
        var min1 = box1.min;
        var small = new Vector3(Mathf.Min(min0.x, min1.x), Mathf.Min(min0.y, min1.y), Mathf.Min(min0.z, min1.z));

        var max0 = box0.max;
        var max1 = box1.max;
        var big = new Vector3(Mathf.Max(max0.x, max1.x), Mathf.Max(max0.y, max1.y), Mathf.Max(max0.z, max1.z));

        return new AABB(small, big);
    }
}


public class HitRecord
{
    public Vector3 p;
    public Vector3 normal;
    public float t;
    public ObjectMaterial objMaterial;
    public bool frontFace;


    public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
    {
        frontFace = Vector3.Dot(ray.direction, outwardNormal) < 0f;
        normal = frontFace ? outwardNormal : -outwardNormal;
    }


}

public class HittableList : Hittable
{
    private List<Hittable> listHittable = new List<Hittable>();


    public void Add(Hittable hittable)
    {
        listHittable.Add(hittable);
    }

    public void Clear()
    {
        listHittable.Clear();
    }


    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        HitRecord tempRec = new HitRecord();
        bool hitAnything = false;
        float closestSoFar = t_max;

        foreach (var hittable in listHittable)
        {
            if (hittable.IsHit(ray, t_min, closestSoFar, ref tempRec))
            {
                hitAnything = true;
                closestSoFar = tempRec.t;
                hitRecord = tempRec;
            }
        }

        return hitAnything;
    }

    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        output = null;

        if (listHittable.Count == 0)
        {
            return false;
        }

        AABB tmpBox;
        bool isFirstBox = true;

        foreach (var hittable in listHittable)
        {
            if (hittable.BoundingBox(time0, time1, out tmpBox) == false)
            {
                return false;
            }

            output = isFirstBox ? tmpBox : SurroundingBox(output, tmpBox);
            isFirstBox = false;
        }

        return true;
    }

}

