using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class RayTracingInOneWeekend : MonoBehaviour
{
    [SerializeField]
    double aspectRatio = 16d / 9d;

    [SerializeField]
    int textureWidth = 160;

    //int textureHeight = textureWidth / aspectRatio;

    [SerializeField]
    double viewportHeight = 2d;




    [SerializeField]
    private Texture2D texResult;









    struct Vec3
    {
        public Vec3(Vector3 vector)
        {
            this.x = vector.x;
            this.y = vector.x;
            this.z = vector.x;
        }

        public Vec3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double x;
        public double y;
        public double z;
    }


    class Ray
    {
        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = new Vec3(origin);
            this.direction = new Vec3(direction);
        }


        public Vec3 origin;
        public Vec3 direction;
    }




    void Start()
    {
        // image
        int textureHeight = (int)(textureWidth / aspectRatio);

        double viewportWidth = aspectRatio * viewportHeight;

        double focalLength = 1d;

        Vec3 original = new Vec3(0d, 0d, 0d);
        Vec3 horizontal = new Vec3(viewportWidth, 0d, 0d);



        texResult = new Texture2D(textureWidth, textureHeight);

        // camera



        // render
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float r = (float)x / (textureWidth - 1);
                float g = (float)y / (textureHeight - 1);
                float b = 0.25f;

                texResult.SetPixel(x, y, new Color(r, g, b, 1f));
            }
        }

        texResult.Apply();
    }




}
