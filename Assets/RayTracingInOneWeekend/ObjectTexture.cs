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
        return (sines < 0f) ? odd.Value(uv, p) : even.Value(uv, p);
    }

}


public class NoiseTexture : ObjectTexture
{
    private PerlinNoise perlinNoise;




    public NoiseTexture()
    {
        perlinNoise = new PerlinNoise();
    }

    public override Color Value(Vector2 uv, Vector3 p)
    {
        return Color.white * perlinNoise.Value(p);
    }
}

