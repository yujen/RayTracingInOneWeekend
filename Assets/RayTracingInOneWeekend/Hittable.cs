using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class HitRecord
{
    public Vector3 p;
    public Vector3 normal;
    public float t;
    public ObjectMaterial objMaterial;
    public bool frontFace;

    /// <summary>
    /// ObjectTexture's UV
    /// </summary>
    public Vector2 uv;



    public void SetFaceNormal(Ray ray, Vector3 outwardNormal)
    {
        frontFace = Vector3.Dot(ray.direction, outwardNormal) < 0f;
        normal = frontFace ? outwardNormal : -outwardNormal;
    }

}


abstract public class Hittable
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
    protected Hittable leftNode, rightNode;
    protected AABB box;


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

    public BVHNode(HittableList list, float time0, float time1)
        : this(list.listHittable, 0, list.listHittable.Count, time0, time1) { }



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


public class Translate : Hittable
{
    protected Hittable hittable;
    protected Vector3 offset;



    public Translate(Hittable hittable, Vector3 offset)
    {
        this.hittable = hittable;
        this.offset = offset;
    }


    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        if (hittable.BoundingBox(time0, time1, out output) == false)
        {
            return false;
        }

        output = new AABB(output.min + offset, output.max + offset);

        return true;
    }

    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        Ray movedRay = new Ray(ray.origin - offset, ray.direction, ray.time);
        if (hittable.IsHit(movedRay, t_min, t_max, ref hitRecord) == false)
        {
            return false;
        }

        hitRecord.p += offset;
        hitRecord.SetFaceNormal(movedRay, hitRecord.normal);

        return true;
    }

}

public class RotateY : Hittable
{
    protected Hittable hittable;
    protected float sin_theta;
    protected float cos_theta;
    protected bool hasBox;
    protected AABB bbox;


    public RotateY(Hittable hittable, float angle)
    {
        this.hittable = hittable;

        float radians = angle * Mathf.Deg2Rad;
        sin_theta = Mathf.Sin(radians);
        cos_theta = Mathf.Cos(radians);

        hasBox = hittable.BoundingBox(0f, 1f, out bbox);

        var min = Vector3.positiveInfinity;
        var max = Vector3.negativeInfinity;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    float x = i * bbox.max.x + (1 - i) * bbox.min.x;
                    float y = j * bbox.max.y + (1 - j) * bbox.min.y;
                    float z = k * bbox.max.z + (1 - k) * bbox.min.z;

                    float newx = cos_theta * x + sin_theta * z;
                    float newz = -sin_theta * x + cos_theta * z;

                    var tester = new Vector3(newx, y, newz);

                    for (int c = 0; c < 3; c++)
                    {
                        min[c] = Mathf.Min(min[c], tester[c]);
                        max[c] = Mathf.Max(max[c], tester[c]);
                    }
                }
            }
        }
        bbox = new AABB(min, max);

    }


    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        output = bbox;

        return hasBox;
    }

    public override bool IsHit(Ray r, float t_min, float t_max, ref HitRecord hitRecord)
    {
        var origin = r.origin;
        var direction = r.direction;

        origin[0] = cos_theta * r.origin[0] - sin_theta * r.origin[2];
        origin[2] = sin_theta * r.origin[0] + cos_theta * r.origin[2];

        direction[0] = cos_theta * r.direction[0] - sin_theta * r.direction[2];
        direction[2] = sin_theta * r.direction[0] + cos_theta * r.direction[2];

        Ray rotatedRay = new Ray(origin, direction, r.time);

        if (hittable.IsHit(rotatedRay, t_min, t_max, ref hitRecord) == false)
        {
            return false;
        }


        var p = hitRecord.p;
        var normal = hitRecord.normal;

        p[0] = cos_theta * hitRecord.p[0] + sin_theta * hitRecord.p[2];
        p[2] = -sin_theta * hitRecord.p[0] + cos_theta * hitRecord.p[2];

        normal[0] = cos_theta * hitRecord.normal[0] + sin_theta * hitRecord.normal[2];
        normal[2] = -sin_theta * hitRecord.normal[0] + cos_theta * hitRecord.normal[2];

        hitRecord.p = p;
        hitRecord.SetFaceNormal(rotatedRay, normal);

        return true;
    }

}

public class FlipFace : Hittable
{
    protected Hittable hittable;


    public FlipFace(Hittable hittable)
    {
        this.hittable = hittable;
    }


    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        return hittable.BoundingBox(time0, time1, out output);
    }

    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        if (hittable.IsHit(ray, t_min, t_max, ref hitRecord) == false)
        {
            return false;
        }

        hitRecord.frontFace = !hitRecord.frontFace;

        return true;

    }
}

