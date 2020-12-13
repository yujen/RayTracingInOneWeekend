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
        float c = perlinNoise.TurbulenceValue(p * scale);
        return new Color(c, c, c, 1f);
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
        float c = 0.5f * (1f + Mathf.Sin(scale * p.z + 10f * perlinNoise.TurbulenceValue(p)));
        return new Color(c, c, c, 1f);
    }
}


public class ImageTexture : ObjectTexture
{
    protected Texture2D image;
    protected int width, height;



    public ImageTexture(Texture2D image)
    {
        if (image == null)
        {
            Debug.LogError("Could not load texture image file");
            return;
        }

        this.image = image;

        width = image.width;
        height = image.height;
    }

    public ImageTexture(string imagePath) : this(Resources.Load<Texture2D>(imagePath)) { }



    public override Color Value(Vector2 uv, Vector3 p)
    {
        // If we have no texture data, then return solid cyan as a debugging aid.
        if (image == null)
        {
            return new Color(0f, 1f, 1f);
        }

        // Clamp input texture coordinates to [0,1] x [1,0]
        float u = Mathf.Clamp01(uv.x);
        float v = Mathf.Clamp01(uv.y);
        //float v = 1f - Mathf.Clamp01(uv.y); // Flip V to image coordinates


        int i = (int)(u * width);
        int j = (int)(v * height);

        // Clamp integer mapping, since actual coordinates should be less than 1.0
        i = (i < width) ? i : (width - 1);
        j = (j < height) ? j : (height - 1);

        return image.GetPixel(i, j);
    }
}

