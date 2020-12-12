using System;
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
    public List<Hittable> listHittable = new List<Hittable>();


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


public class BVHNode : Hittable
{
    public Hittable leftNode, rightNode;
    public AABB box;




    public BVHNode(List<Hittable> listHittable, int start, int end, float time0, float time1)
    {
        int axis = UnityEngine.Random.Range(0, 3);  // 0=x, 1=y, 2=z
        var comparator = (axis == 0) ? new Comparison<Hittable>(BoxXCompare)
                        : (axis == 1) ? new Comparison<Hittable>(BoxYCompare)
                                      : new Comparison<Hittable>(BoxZCompare);


        int objSpan = end - start;
        if (objSpan == 1)
        {
            leftNode = rightNode = listHittable[start];
        }
        else if (objSpan == 2)
        {
            if (comparator(listHittable[start], listHittable[start + 1]) < 0)
            {
                leftNode = listHittable[start];
                rightNode = listHittable[start + 1];
            }
            else
            {
                leftNode = listHittable[start + 1];
                rightNode = listHittable[start];
            }
        }
        else
        {
            listHittable.Sort(comparator);

            int mid = start + objSpan / 2;
            leftNode = new BVHNode(listHittable, start, mid, time0, time1);
            rightNode = new BVHNode(listHittable, mid, end, time0, time1);
        }


        AABB box_left = null, box_right = null;

        if (!leftNode.BoundingBox(time0, time1, out box_left)
           || !rightNode.BoundingBox(time0, time1, out box_right)
        )
        {
            Debug.LogError("No bounding box in bvh_node constructor");
        }

        box = SurroundingBox(box_left, box_right);

    }

    /*
    public BVHNode(HittableList list, float time0, float time1)
    {
        BVHNode(list.listHittable, 0, list.listHittable.Count, time0, time1);
    }
    */



    int BoxXCompare(Hittable a, Hittable b)
    {
        return BoxCompare(a, b, 0);
    }

    int BoxYCompare(Hittable a, Hittable b)
    {
        return BoxCompare(a, b, 1);
    }

    int BoxZCompare(Hittable a, Hittable b)
    {
        return BoxCompare(a, b, 2);
    }

    int BoxCompare(Hittable a, Hittable b, int axis)
    {
        AABB box_a = null, box_b = null;

        if (a.BoundingBox(0f, 0f, out box_a) == false || b.BoundingBox(0f, 0f, out box_b) == false)
        {
            Debug.LogError("No bounding box in BVHNode constructor");
        }

        return box_a.min[axis].CompareTo(box_b.min[axis]);
    }


    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        output = box;

        return true;
    }

    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        if (box.IsHit(ray, t_min, t_max) == false)
        {
            return false;
        }

        bool isLeftNodeHit = leftNode.IsHit(ray, t_min, t_max, ref hitRecord);
        bool isRightNodeHit = rightNode.IsHit(ray, t_min, isLeftNodeHit ? hitRecord.t : t_max, ref hitRecord);

        return (isLeftNodeHit || isRightNodeHit);
    }
}
