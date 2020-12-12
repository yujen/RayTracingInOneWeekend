using System.Collections;
using System.Collections.Generic;
using UnityEngine;



abstract public class ObjectTexture
{

    abstract public Color Value(Vector2 uv, Vector3 p);
}



public class SolidColor : ObjectTexture
{
    protected Color colorValue;



    public SolidColor(Color c)
    {
        colorValue = c;
    }


    public override Color Value(Vector2 uv, Vector3 p)
    {
        return colorValue;
    }

}


public class CheckerTexture : ObjectTexture
{
    protected ObjectTexture even, odd;




    public CheckerTexture(ObjectTexture even, ObjectTexture odd)
    {
        this.even = even;
        this.odd = odd;
    }

    public CheckerTexture(Color even, Color odd) : this(new SolidColor(even), new SolidColor(odd)) { }




    public override Color Value(Vector2 uv, Vector3 p)
    {
        float sines = Mathf.Sin(10f * p.x) * Mathf.Sin(10f * p.y) * Mathf.Sin(10f * p.z);
        if (sines < 0f)
        {
            return odd.Value(uv, p);
        }
        else
        {
            return even.Value(uv, p);
        }

    }
}

