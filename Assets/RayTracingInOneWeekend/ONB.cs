using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Orthonormal Basis
/// </summary>
public class ONB
{
    private Vector3[] axis = new Vector3[3];




    public Vector3 u
    {
        get
        {
            return axis[0];
        }
    }

    public Vector3 v
    {
        get
        {
            return axis[1];
        }
    }

    public Vector3 w
    {
        get
        {
            return axis[2];
        }
    }


    public Vector3 Local(Vector3 a)
    {
        return a.x * u + a.y * v + a.z * w;
    }



    public void BuildFromW(Vector3 normal)
    {
        axis[2] = normal.normalized;

        Vector3 a = Mathf.Abs(w.x) > 0.9f ? new Vector3(0f, 1f, 0f) : new Vector3(1f, 0f, 0f);

        axis[1] = Vector3.Cross(w, a).normalized;
        axis[0] = Vector3.Cross(w, v);
    }

}
