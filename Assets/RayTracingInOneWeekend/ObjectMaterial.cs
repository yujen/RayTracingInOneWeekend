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

    public Vector3 Reflect(Vector3 v, Vector3 n)
    {
        return v - (2 * Vector3.Dot(v, n) * n);
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


    public MetalMaterial(Color albedo)
    {
        this.albedo = albedo;
    }

}

public class FuzzyMetalMaterial : ObjectMaterial
{
    public Color albedo;
    public float fuzz;


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        var reflected = Reflect(inRay.direction.normalized, hitRecord.normal);
        scatteredrRay = new Ray(hitRecord.p, reflected + Random.insideUnitSphere * fuzz);
        attenuation = albedo;
        return (Vector3.Dot(scatteredrRay.direction, hitRecord.normal) > 0f);
    }


    public FuzzyMetalMaterial(Color albedo, float fuzz = 0.7f)
    {
        this.albedo = albedo;
        this.fuzz = (fuzz < 1f) ? fuzz : 1f;
    }

}


public class DielectricMaterial : ObjectMaterial
{
    public float indexOfRefraction;


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        attenuation = Color.white;

        float refractionRatio = hitRecord.frontFace ? (1f / indexOfRefraction) : indexOfRefraction;
        Vector3 unitDirection = inRay.direction.normalized;
        Vector3 refracted = Refract(unitDirection, hitRecord.normal, refractionRatio);
        scatteredrRay = new Ray(hitRecord.p, refracted);

        return true;
    }


    Vector3 Refract(Vector3 uv, Vector3 n, float etai_over_etat)
    {
        float cos_theta = Mathf.Min(Vector3.Dot(-uv, n), 1f);
        Vector3 r_out_perp = etai_over_etat * (uv + cos_theta * n);
        Vector3 r_out_parallel = -Mathf.Sqrt(Mathf.Abs(1f - r_out_perp.sqrMagnitude)) * n;
        return r_out_perp + r_out_parallel;
    }

    public DielectricMaterial(float indexOfRefraction = 1.5f)
    {
        this.indexOfRefraction = indexOfRefraction;
    }

}