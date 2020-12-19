using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class ScatterRecord
{
    public Ray specluarRay;
    public bool isSpecular;
    public Color attenuation;
    public PDF pdf;
}

abstract public class ObjectMaterial
{

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


    abstract public bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec);

    /// <summary>
    /// Probability Density Function for importance sampling
    /// </summary>
    abstract public float ScatteringPDF(Ray inRay, HitRecord hitRecord, Ray scatteredRay);


    virtual public Color Emitted(Ray inRay, HitRecord hitRecord)
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


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec)
    {
        scatterRec = new ScatterRecord();
        scatterRec.specluarRay = null;
        scatterRec.isSpecular = false;
        scatterRec.attenuation = albedo.Value(hitRecord.uv, hitRecord.p);
        scatterRec.pdf = new CosinePDF(hitRecord.normal);

        return true;
    }

    public override float ScatteringPDF(Ray inRay, HitRecord hitRecord, Ray scatteredRay)
    {
        float cosine = Vector3.Dot(hitRecord.normal, scatteredRay.direction.normalized);
        return (cosine < 0f) ? 0f : (cosine / Mathf.PI);
    }

}



public class MetalMaterial : ObjectMaterial
{
    protected Color albedo;



    public MetalMaterial(Color albedo)
    {
        this.albedo = albedo;
    }


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec)
    {
        var reflected = Reflect(inRay.direction.normalized, hitRecord.normal);

        scatterRec = new ScatterRecord();
        scatterRec.specluarRay = new Ray(hitRecord.p, reflected, inRay.time);
        scatterRec.isSpecular = true;
        scatterRec.attenuation = albedo;
        scatterRec.pdf = null;

        return true;
    }

    public override float ScatteringPDF(Ray inRay, HitRecord hitRecord, Ray scatteredRay)
    {
        throw new System.NotImplementedException();
    }
}


public class FuzzyMetalMaterial : MetalMaterial
{
    protected float fuzz;


    public FuzzyMetalMaterial(Color albedo, float fuzz = 0.7f) : base(albedo)
    {
        this.fuzz = (fuzz < 1f) ? fuzz : 1f;
    }


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec)
    {
        var reflected = Reflect(inRay.direction.normalized, hitRecord.normal);

        scatterRec = new ScatterRecord();
        scatterRec.specluarRay = new Ray(hitRecord.p, reflected + Random.insideUnitSphere * fuzz, 0f);
        scatterRec.isSpecular = true;
        scatterRec.attenuation = albedo;
        scatterRec.pdf = null;

        return true;
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


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec)
    {
        scatterRec = new ScatterRecord();
        scatterRec.isSpecular = true;
        scatterRec.attenuation = Color.white;
        scatterRec.pdf = null;

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

        scatterRec.specluarRay = new Ray(hitRecord.p, direction, inRay.time);

        return true;
    }

    public override float ScatteringPDF(Ray inRay, HitRecord hitRecord, Ray scatteredRay)
    {
        throw new System.NotImplementedException();
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


    public override bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec)
    {
        scatterRec = null;

        return false;
    }

    public override float ScatteringPDF(Ray inRay, HitRecord hitRecord, Ray scatteredRay)
    {
        throw new System.NotImplementedException();
    }
    public override Color Emitted(Ray inRay, HitRecord hitRecord)
    {
        if (hitRecord.frontFace)
        {
            return emit.Value(hitRecord.uv, hitRecord.p);
        }
        else
        {
            return Color.black;
        }
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



    public override bool Scatter(Ray inRay, HitRecord hitRecord, out ScatterRecord scatterRec)
    {
        scatterRec = new ScatterRecord();
        scatterRec.specluarRay = new Ray(hitRecord.p, Random.insideUnitSphere, inRay.time);
        scatterRec.isSpecular = true;
        scatterRec.attenuation = albedo.Value(hitRecord.uv, hitRecord.p);
        scatterRec.pdf = null;

        return true;
    }

    public override float ScatteringPDF(Ray inRay, HitRecord hitRecord, Ray scatteredRay)
    {
        throw new System.NotImplementedException();
    }
}


