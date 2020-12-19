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
