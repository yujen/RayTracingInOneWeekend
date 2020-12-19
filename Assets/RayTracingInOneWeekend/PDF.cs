using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Probability Density Function
/// </summary>
abstract public class PDF
{


    abstract public float Value(Vector3 direction);


    abstract public Vector3 Generate();

}


public class CosinePDF : PDF
{
    protected ONB uvw;


    public CosinePDF(Vector3 w)
    {
        uvw = new ONB();
        uvw.BuildFromW(w);
    }



    public override Vector3 Generate()
    {
        return uvw.Local(Utils.RandomCosineDirection);
    }

    public override float Value(Vector3 direction)
    {
        var cosine = Vector3.Dot(direction.normalized, uvw.w);
        return (cosine <= 0f) ? 0f : cosine / Mathf.PI;
    }
}


public class HittablePDF : PDF
{
    protected Hittable hittable;
    protected Vector3 origin;



    public HittablePDF(Hittable hittable, Vector3 origin)
    {
        this.hittable = hittable;
        this.origin = origin;
    }


    public override Vector3 Generate()
    {
        return hittable.RandomPDF(origin);
    }

    public override float Value(Vector3 direction)
    {
        return hittable.ValuePDF(origin, direction);
    }
}


public class MixturePDF : PDF
{
    private PDF[] listPDF = new PDF[2];


    public MixturePDF(PDF p0, PDF p1)
    {
        listPDF[0] = p0;
        listPDF[1] = p1;
    }

    public override Vector3 Generate()
    {
        if (Random.value < 0.5f)
        {
            return listPDF[0].Generate();
        }
        else
        {
            return listPDF[1].Generate();
        }
    }

    public override float Value(Vector3 direction)
    {
        return 0.5f * listPDF[0].Value(direction) + 0.5f * listPDF[1].Value(direction);
    }

}

