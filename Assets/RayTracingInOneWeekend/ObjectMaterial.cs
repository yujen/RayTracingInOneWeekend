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

    public Vector3 Refract(Vector3 uv, Vector3 n, float etai_over_etat)
    {
        float cos_theta = Mathf.Min(Vector3.Dot(-uv, n), 1f);
        Vector3 r_out_perp = etai_over_etat * (uv + cos_theta * n);
        Vector3 r_out_parallel = -Mathf.Sqrt(Mathf.Abs(1f - r_out_perp.sqrMagnitude)) * n;
        return r_out_perp + r_out_parallel;
    }


    abstract public bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay);


    virtual public Color Emitted(Vector2 uv, Vector3 p)
    {
        return Color.black;
    }

}



public class LambertainMaterial : ObjectMaterial
{
    protected ObjectTexture albedo;



    public LambertainMaterial(ObjectTexture t)
    {
        this.albedo = t;
    }

    public LambertainMaterial(Color albedo) : this(new SolidColor(albedo)) { }


    override public bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        var scatterDirection = hitRecord.normal + Random.onUnitSphere;

        // Catch degenerate scatter direction
        if (IsNearZero(scatterDirection))
        {
            scatterDirection = hitRecord.normal;
        }

        scatteredrRay = new Ray(hitRecord.p, scatterDirection, inRay.time);
        attenuation = albedo.Value(hitRecord.uv, hitRecord.p);
        return true;
    }

}



public class MetalMaterial : ObjectMaterial
{
    protected Color albedo;



    public MetalMaterial(Color albedo)
    {
        this.albedo = albedo;
    }


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        var reflected = Reflect(inRay.direction.normalized, hitRecord.normal);
        scatteredrRay = new Ray(hitRecord.p, reflected, inRay.time);
        attenuation = albedo;
        return (Vector3.Dot(scatteredrRay.direction, hitRecord.normal) > 0f);
    }

}


public class FuzzyMetalMaterial : MetalMaterial
{
    protected float fuzz;


    public FuzzyMetalMaterial(Color albedo, float fuzz = 0.7f) : base(albedo)
    {
        this.fuzz = (fuzz < 1f) ? fuzz : 1f;
    }


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        var reflected = Reflect(inRay.direction.normalized, hitRecord.normal);
        scatteredrRay = new Ray(hitRecord.p, reflected + Random.insideUnitSphere * fuzz, inRay.time);
        attenuation = albedo;
        return (Vector3.Dot(scatteredrRay.direction, hitRecord.normal) > 0f);
    }

}


public class DielectricMaterial : ObjectMaterial
{
    /// <summary>
    /// Refractive index
    /// </summary>
    protected float indexOfRefraction;


    public DielectricMaterial(float indexOfRefraction = 1.5f)
    {
        this.indexOfRefraction = indexOfRefraction;
    }


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        attenuation = Color.white;

        float refraction_ratio = hitRecord.frontFace ? (1f / indexOfRefraction) : indexOfRefraction;
        Vector3 unit_direction = inRay.direction.normalized;

        float cos_theta = Mathf.Min(Vector3.Dot(-unit_direction, hitRecord.normal), 1f);
        float sin_theta = Mathf.Sqrt(1f - cos_theta * cos_theta);
        bool cannot_refract = refraction_ratio * sin_theta > 1f;

        //
        Vector3 direction;
        if (cannot_refract || Reflectance(cos_theta, refraction_ratio) > Random.value)
        {
            direction = Reflect(unit_direction, hitRecord.normal);
        }
        else
        {
            direction = Refract(unit_direction, hitRecord.normal, refraction_ratio);
        }

        scatteredrRay = new Ray(hitRecord.p, direction, inRay.time);

        return true;
    }


    /// <summary>
    /// Schlick's approximation for reflectance.
    /// </summary>
    private float Reflectance(float cosine, float ref_idx)
    {
        float r0 = (1f - ref_idx) / (1f + ref_idx);
        r0 = r0 * r0;
        return r0 + (1f - r0) * Mathf.Pow((1f - cosine), 5f);
    }


}


public class DiffuseLight : ObjectMaterial
{
    protected ObjectTexture emit;


    public DiffuseLight(Color c)
    {
        emit = new SolidColor(c);
    }


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        attenuation = Color.black;
        scatteredrRay = null;

        return false;
    }

    public override Color Emitted(Vector2 uv, Vector3 p)
    {
        return emit.Value(uv, p);
    }

}


public class Isotropic : ObjectMaterial
{
    protected ObjectTexture albedo;



    public Isotropic(ObjectTexture t)
    {
        albedo = t;
    }

    public Isotropic(Color c) : this(new SolidColor(c)) { }



    public override bool Scatter(Ray inRay, HitRecord hitRecord, out Color attenuation, out Ray scatteredrRay)
    {
        attenuation = albedo.Value(hitRecord.uv, hitRecord.p);
        scatteredrRay = new Ray(hitRecord.p, Random.insideUnitSphere, inRay.time);

        return true;
    }
}


