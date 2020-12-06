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
    int samplesPerPixel = 8;

    [SerializeField]
    int maxDepth = 8;




    [SerializeField, ReadOnly]
    private Texture2D texResult;




    Vector3 RandomInsideUnitHemisphere(Vector3 normal)
    {
        var unitSphere = Random.insideUnitSphere;
        if (Vector3.Dot(unitSphere, normal) > 0f)
        {
            return unitSphere;
        }
        else
        {
            return -unitSphere;
        }
    }

    Color RayColor(Ray ray, Hittable world, int depth)
    {
        // If we've exceeded the ray bounce limit, no more light is gathered
        if (depth <= 0)
        {
            return Color.black;
        }

        HitRecord hitRecord = null;
        // t_min=0.0001f to fix shadow acne
        if (world.IsHit(ray, 0.0001f, float.MaxValue, ref hitRecord))
        {
            Color attenuation;
            Ray scattered;
            if (hitRecord.objMaterial.Scatter(ray, hitRecord, out attenuation, out scattered))
            {
                return attenuation * RayColor(scattered, world, depth - 1);
            }
            else
            {
                return Color.black;
            }

        }

        // background
        var unitDirection = ray.direction.normalized;
        float offset = (unitDirection.y + 1f) * 0.5f;
        return (1f - offset) * Color.white + offset * new Color(0.5f, 0.7f, 1f);
    }

    Color RayColor2(Ray ray, Hittable world)
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




    /// <summary>
    /// Returns a random real in [0,1).
    /// </summary>
    float GetRandomNum()
    {
        return Random.Range(0f, 0.999999f);
    }


    void WriteColor(Texture2D tex, int x, int y, Color pixelColor, int samplesPerPixel)
    {
        // Divide the color by the number of samples.
        float scale = 1f / (float)samplesPerPixel;
        pixelColor *= scale;

        /*
        // gamma-correct for gamma=2.0
        pixelColor = new Color(
            Mathf.Sqrt(scale * pixelColor.r), 
            Mathf.Sqrt(scale * pixelColor.g), 
            Mathf.Sqrt(scale * pixelColor.b));
        */


        // Write the translated [0,1] value of each color component.
        tex.SetPixel(x, y, pixelColor);
    }


    HittableList RandomScene()
    {
        var world = new HittableList();

        var matGround = new LambertainMaterial(new Color(0.5f, 0.5f, 0.5f));
        world.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, matGround));


        for (int i = -11; i < 11; i++)
        {
            for (int j = -11; j < 11; j++)
            {
                var chooseMat = Random.value;
                var center = new Vector3((float)i + 0.9f * Random.value, 0.2f, (float)j + 0.9f * Random.value);


            }
        }

        var mat1 = new DielectricMaterial(1.5f);
        world.Add(new Sphere(new Vector3(0f, 1f, 0f), 1f, mat1));

        var mat2 = new LambertainMaterial(new Color(0.4f, 0.2f, 0.1f));
        world.Add(new Sphere(new Vector3(-4f, 1f, 0f), 1.0f, mat2));

        var mat3 = new FuzzyMetalMaterial(new Color(0.7f, 0.6f, 0.5f), 0f);
        world.Add(new Sphere(new Vector3(4f, 1f, 0f), 1f, mat3));

        return world;
    }

    void Start()
    {
        // image
        int textureWidth = textureWidthHeight.x;
        int textureHeight = textureWidthHeight.y;
        texResult = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false, true);

        // world
        /*var matGround = new LambertainMaterial(new Color(0.8f, 0.8f, 0f));
        var matCenter = new LambertainMaterial(new Color(0.1f, 0.2f, 0.5f));
        //var matLeft = new MetalMaterial(new Color(0.8f, 0.8f, 0.8f));
        var matLeft = new DielectricMaterial(1.5f);
        var matRight = new FuzzyMetalMaterial(new Color(0.8f, 0.6f, 0.2f), 0.7f);

        HittableList world = new HittableList();
        world.Add(new Sphere(new Vector3(0f, -100.5f, 0f), 100f, matGround));
        world.Add(new Sphere(new Vector3(0f, 0f, 1f), 0.5f, matCenter));
        world.Add(new Sphere(new Vector3(-1f, 0f, 0f), 0.5f, matLeft));
        world.Add(new Sphere(new Vector3(-1f, 0f, 0f), -0.4f, matLeft));
        world.Add(new Sphere(new Vector3(1f, 0f, 0f), 0.5f, matRight));
        */

        HittableList world = RandomScene();

        // camera
        var cam = GetComponent<RayCamera>();
        cam = cam ? cam : new RayCamera();
        cam.Setup((float)textureWidth / (float)textureHeight);


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
                    pixelColor += RayColor(ray, world, maxDepth);
                }
                WriteColor(texResult, x, y, pixelColor, samplesPerPixel);
            }
        }

        texResult.Apply();

    }





}
