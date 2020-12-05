using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;





public class RayTracingInOneWeekend : MonoBehaviour
{
    //[SerializeField]
    //int textureWidth = 160;
    //[SerializeField]
    //int textureHeight = 90;

    [SerializeField]
    Vector2Int textureWidthHeight = new Vector2Int(320, 180);

    [SerializeField]
    int samplesPerPixel = 10;





    [SerializeField, ReadOnly]
    private Texture2D texResult;







    class ColorDouble
    {
        public double r;
        public double g;
        public double b;

        public ColorDouble(double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color ToColor()
        {
            return new Color((float)r, (float)g, (float)b);
        }
    }



    ColorDouble RayColor(Ray ray, Hittable world)
    {
        HitRecord hitRecord = null;

        if (world.IsHit(ray, 0f, float.MaxValue, ref hitRecord))
        {
            var normal = hitRecord.normal;
            return new ColorDouble((normal.x + 1d) * 0.5d, (normal.y + 1d) * 0.5d, (normal.z + 1d) * 0.5d);
        }

        // background
        var unitDirection = ray.direction.normalized;
        float offset = (unitDirection.y + 1f) * 0.5f;
        var color = (1f - offset) * Color.white + offset * new Color(0.5f, 0.7f, 1f);

        return new ColorDouble(color.r, color.g, color.b);
    }



    float GetRandomNum()
    {
        return 0f;
        return Random.Range(0f, 0.99999f);
    }


    void WriteColor(Texture2D tex, int x, int y, ColorDouble pixelColor, int samplesPerPixel)
    {
        double r = pixelColor.r;
        double g = pixelColor.g;
        double b = pixelColor.b;

        // Divide the color by the number of samples.
        double scale = 1d / (double)samplesPerPixel;
        r *= scale;
        g *= scale;
        b *= scale;


        // Write the translated [0,1] value of each color component.
        tex.SetPixel(x, y, new Color((float)r, (float)g, (float)b));
    }



    void Start()
    {
        // image
        int textureWidth = textureWidthHeight.x;
        int textureHeight = textureWidthHeight.y;
        texResult = new Texture2D(textureWidth, textureHeight);

        // world
        HittaleList world = new HittaleList(); ;
        world.Add(new Sphere(new Vector3(0f, 0f, -1f), 0.5f));
        world.Add(new Sphere(new Vector3(0f, -100.5f, -1f), 100f));

        // camera
        var cam = new RayCamera(textureWidth, textureHeight);


        // render
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float u = ((float)x + GetRandomNum()) / (textureWidth - 1);
                float v = ((float)y + GetRandomNum()) / (textureHeight - 1);
                ColorDouble pixelColor = new ColorDouble(0d, 0d, 0d);

                for (int i = 0; i < samplesPerPixel; i++)
                {
                    var ray = cam.GetRay(u, v);

                    ColorDouble c = RayColor(ray, world);
                    pixelColor.r += c.r;
                    pixelColor.g += c.g;
                    pixelColor.b += c.b;
                }
                WriteColor(texResult, x, y, pixelColor, samplesPerPixel);
            }
        }

        texResult.Apply();

    }


    


}
