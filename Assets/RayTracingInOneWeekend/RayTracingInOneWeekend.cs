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






    Color RayColor(Ray ray, Hittable world)
    {
        HitRecord hitRecord = null;

        if (world.IsHit(ray, 0f, float.MaxValue, ref hitRecord))
        {
            var normal = hitRecord.normal;
            return new Color((normal.x + 1f), (normal.y + 1f), (normal.z + 1f)) * 0.5f;
        }

        // background
        var unitDirection = ray.direction.normalized;
        float offset = (unitDirection.y + 1f) * 0.5f;
        return (1f - offset) * Color.white + offset * new Color(0.5f, 0.7f, 1f);
    }



    float GetRandomNum()
    {
        return Random.Range(0f, 0.999999f);
    }


    void WriteColor(Texture2D tex, int x, int y, Color pixelColor, int samplesPerPixel)
    {
        // Divide the color by the number of samples.
        float scale = 1f / (float)samplesPerPixel;
        pixelColor *= scale;


        // Write the translated [0,1] value of each color component.
        tex.SetPixel(x, y, pixelColor);
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
                Color pixelColor = Color.black;

                for (int i = 0; i < samplesPerPixel; i++)
                {
                    float u = ((float)x + GetRandomNum()) / (textureWidth - 1);
                    float v = ((float)y + GetRandomNum()) / (textureHeight - 1);

                    var ray = cam.GetRay(u, v);
                    pixelColor += RayColor(ray, world);
                }
                WriteColor(texResult, x, y, pixelColor, samplesPerPixel);
            }
        }

        texResult.Apply();

    }


    


}
