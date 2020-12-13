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
    protected PerlinNoise perlinNoise;
    protected float scale;



    public NoiseTexture(float scale = 4f)
    {
        perlinNoise = new PerlinNoise();
        this.scale = scale;
    }

    public override Color Value(Vector2 uv, Vector3 p)
    {
        //return Color.white * perlinNoise.Value(p * scale);
        //return Color.white * perlinNoise.TrilinearInterpValue(p * scale);
        //return Color.white * 0.5f * (1f + perlinNoise.PerlinInterpValue(p * scale));
        return Color.white * perlinNoise.TurbulenceValue(p * scale);
    }
}

/// <summary>
/// Marble-like texture
/// </summary>
public class MarbleTexture : NoiseTexture
{

    public MarbleTexture(float scale = 4f) : base(scale) { }


    public override Color Value(Vector2 uv, Vector3 p)
    {
        return Color.white * 0.5f * (1f + Mathf.Sin(scale * p.z + 10f * perlinNoise.TurbulenceValue(p)));
    }
}


