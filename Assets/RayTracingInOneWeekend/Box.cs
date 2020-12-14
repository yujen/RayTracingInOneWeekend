using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Box : Hittable
{
    protected Vector3 box_min, box_max;
    protected HittableList sides = new HittableList();




    public Box(Vector3 p0, Vector3 p1, ObjectMaterial objMaterial)
    {
        box_min = p0;
        box_max = p1;

        sides.Add(new RectangleXY(p0.x, p1.x, p0.y, p1.y, p1.z, objMaterial));
        sides.Add(new RectangleXY(p0.x, p1.x, p0.y, p1.y, p0.z, objMaterial));

        sides.Add(new RectangleXZ(p0.x, p1.x, p0.z, p1.z, p1.y, objMaterial));
        sides.Add(new RectangleXZ(p0.x, p1.x, p0.z, p1.z, p0.y, objMaterial));

        sides.Add(new RectangleYZ(p0.y, p1.y, p0.z, p1.z, p1.x, objMaterial));
        sides.Add(new RectangleYZ(p0.y, p1.y, p0.z, p1.z, p0.x, objMaterial));

    }


    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        output = new AABB(box_min, box_max);
        return true;
    }

    public override bool IsHit(Ray ray, float t_min, float t_max, ref HitRecord hitRecord)
    {
        return sides.IsHit(ray, t_min, t_max, ref hitRecord);
    }
}
