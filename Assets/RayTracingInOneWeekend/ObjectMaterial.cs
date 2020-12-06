using System.Collections;
using System.Collections.Generic;
using UnityEngine;




abstract public class ObjectMaterial
{
    /// <summary>
    /// Return true if the vector is close to zero in all dimensions.
    /// </summary>
    public bool IsNearZero(Vector3 v)
    {
        float s = 0.0001f;
        return (Mathf.Abs(v.x) < s) && (Mathf.Abs(v.y) < s) && (Mathf.Abs(v.z) < s);
    }


    abstract public bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay);

}



public class LambertainMaterial : ObjectMaterial
{
    public Color albedo;


    override public bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        var scatterDirection = hitRecord.normal + Random.onUnitSphere;

        // Catch degenerate scatter direction
        if (IsNearZero(scatterDirection))
        {
            scatterDirection = hitRecord.normal;
        }


        scatteredrRay = new Ray(hitRecord.p, scatterDirection);
        attenuation = albedo;
        return true;
    }

    public LambertainMaterial(Color albedo)
    {
        this.albedo = albedo;
    }

}



public class MetalMaterial : ObjectMaterial
{
    public Color albedo;


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        var reflected = Reflect(inRay.direction.normalized, hitRecord.normal);
        scatteredrRay = new Ray(hitRecord.p, reflected);
        attenuation = albedo;
        return (Vector3.Dot(scatteredrRay.direction, hitRecord.normal) > 0f);
    }


    public Vector3 Reflect(Vector3 v, Vector3 n)
    {
        return v - (2 * Vector3.Dot(v, n) * n);
    }


    public MetalMaterial(Color albedo)
    {
        this.albedo = albedo;
    }

}
