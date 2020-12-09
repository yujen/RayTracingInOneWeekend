using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class Hittable
{
    abstract public bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord);

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
}

