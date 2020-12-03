using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class RayTracingInOneWeekend : MonoBehaviour
{
    [SerializeField]
    float aspectRatio = 16f / 9f;

    [SerializeField]
    int textureWidth = 160;

    //int textureHeight = textureWidth / aspectRatio;






    [SerializeField]
    private Texture2D texResult;











    Color RayColor(Ray ray, Hittable world)
    {
        HitRecord hitRecord = null;
        if (world.IsHit(ray, 0f, float.MaxValue, ref hitRecord))
        {
            var normal = hitRecord.normal;
            return new Color(normal.x + 1f, normal.y + 1f, normal.z + 1f) * 0.5f;
        }

        // background
        var unitDirection = ray.direction.normalized;
        float offset = (unitDirection.y + 1f) * 0.5f;

        return (1f - offset) * Color.white + offset * new Color(0.5f, 0.7f, 1f);
    }


    float HitSphere(Vector3 center, float radius, Ray ray)
    {
        Vector3 oc = ray.origin - center;
        float a = ray.direction.sqrMagnitude;
        float half_b = Vector3.Dot(oc, ray.direction);
        float c = oc.sqrMagnitude - radius * radius;
        float discriminant = half_b * half_b - a * c;

        if (discriminant >= 0f)
        {
            return (-half_b - Mathf.Sqrt(discriminant)) / a;
        }
        else
        {
            return -1f;
        }
    }



    void Start()
    {
        // image
        int textureHeight = (int)(textureWidth / aspectRatio);
        texResult = new Texture2D(textureWidth, textureHeight);

        // world
        HittaleList world = new HittaleList(); ;
        world.Add(new Sphere(new Vector3(0f, 0f, -1f), 0.5f));
        world.Add(new Sphere(new Vector3(0f, -100.5f, -1f), 100f));

        // camera
        float viewportHeight = 2f;
        float viewportWidth = aspectRatio * viewportHeight;

        float focalLength = 1f;

        Vector3 origin = Vector3.zero;
        Vector3 horizontal = new Vector3(viewportWidth, 0f, 0f);
        Vector3 vertical = new Vector3(0f, viewportHeight, 0f);
        Vector3 lowerLeftCorner = origin - (horizontal / 2f) - (vertical / 2f) - new Vector3(0f, 0f, focalLength);



        // render
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float u = (float)x / (textureWidth - 1);
                float v = (float)y / (textureHeight - 1);

                var ray = new Ray(origin, (lowerLeftCorner + horizontal * u + vertical * v) - origin);
                var color = RayColor(ray, world);

                texResult.SetPixel(x, y, color);
            }
        }

        texResult.Apply();
    }




}
