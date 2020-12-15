using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ConstantMedium : Hittable
{
    protected Hittable boundary;
    protected float neg_inv_density;
    protected ObjectMaterial phase_function;


    public ConstantMedium(Hittable boundary, float density, ObjectTexture objectTexture)
    {
        this.boundary = boundary;
        this.neg_inv_density = (-1f / density);
        this.phase_function = new Isotropic(objectTexture);
    }

    public ConstantMedium(Hittable boundary, float density, Color c)
    {
        this.boundary = boundary;
        this.neg_inv_density = (-1f / density);
        this.phase_function = new Isotropic(c);
    }



    public override bool BoundingBox(float time0, float time1, out AABB output)
    {
        return boundary.BoundingBox(time0, time1, out output);
    }

    public override bool IsHit(Ray r, float t_min, float t_max, ref HitRecord rec)
    {
        // Print occasional samples when debugging. To enable, set enableDebug true.
        bool enableDebug = false;
        bool debugging = enableDebug && Random.value < 0.0001f;


        //
        HitRecord rec1 = null, rec2 = null;

        if (boundary.IsHit(r, float.NegativeInfinity, float.PositiveInfinity, ref rec1) == false)
        {
            return false;
        }
        if (boundary.IsHit(r, rec1.t + 0.0001f, float.PositiveInfinity, ref rec2) == false)
        {
            return false;
        }


        if (debugging)
        {
            Debug.LogError($"t_min={rec1.t}, t_max={rec2.t}");
        }


        rec1.t = (rec1.t < t_min) ? t_min : rec1.t;
        rec2.t = (rec2.t > t_max) ? t_max : rec2.t;

        if (rec1.t >= rec2.t)
        {
            return false;
        }


        if (rec1.t < 0f)
        {
            rec1.t = 0f;
        }


        var ray_length = r.direction.magnitude;
        var distance_inside_boundary = (rec2.t - rec1.t) * ray_length;
        var hit_distance = neg_inv_density * Mathf.Log(Random.value);

        if (hit_distance > distance_inside_boundary)
        {
            return false;
        }


        rec.t = rec1.t + hit_distance / ray_length;
        rec.p = r.At(rec.t);

        if (debugging)
        {
            Debug.LogError($"hit_distance={hit_distance}, rec.t={rec.t}, rec.p={rec.p}");
        }

        rec.normal = new Vector3(1f, 0f, 0f);  // arbitrary
        rec.frontFace = true;     // also arbitrary
        rec.objMaterial = phase_function;

        return true;
    }

}

