
using System.IO;
using UnityEngine;



public enum Scene
{
    RandomSphereScene,
    TwoSphereScene,
    TwoPerlinNoiseSphereScene,
    EarthScene,
    SimpleLightScene,
    CornellBoxScene,
}


public class RayTracingInOneWeekend : MonoBehaviour
{
    [Header("RayTracing Parameters")]

    [SerializeField]
    private Scene scene;

    [SerializeField]
    private Color backgroundColor = new Color(0.7f, 0.8f, 1f, 1f);

    [SerializeField]
    private Vector2Int textureWidthHeight = new Vector2Int(320, 180);

    [SerializeField, Range(1, 500)]
    private int samplesPerPixel = 8;

    [SerializeField, Range(1, 50)]
    private int maxDepth = 8;

    [SerializeField]
    private Texture2D textureResult;





    Color RayColor(Ray ray, Hittable world, int depth)
    {
        // If we've exceeded the ray bounce limit, no more light is gathered
        if (depth <= 0)
        {
            return Color.black;
        }

        HitRecord hitRecord = null;
        // t_min=0.0001f to fix shadow acne
        if (world.IsHit(ray, 0.0001f, float.PositiveInfinity, ref hitRecord))
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

    Color RayColor(Ray ray, Color background, Hittable world, int depth)
    {
        // If we've exceeded the ray bounce limit, no more light is gathered
        if (depth <= 0)
        {
            return Color.black;
        }


        HitRecord hitRecord = null;

        // If the ray hits nothing, return the background color.
        if (world.IsHit(ray, 0.0001f, float.PositiveInfinity, ref hitRecord))
        {
            Color attenuation;
            Ray scattered;
            Color emitted = hitRecord.objMaterial.Emitted(hitRecord.uv, hitRecord.p);

            if (hitRecord.objMaterial.Scatter(ray, hitRecord, out attenuation, out scattered))
            {
                return emitted + attenuation * RayColor(scattered, background, world, depth - 1);
            }
            else
            {
                return emitted;
            }
        }
        else
        {
            return background;
        }

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


    HittableList RandomSphereScene()
    {
        var listObj = new HittableList();

        //
        var texChecker = new CheckerTexture(new Color(0.2f, 0.3f, 0.1f), new Color(0.9f, 0.9f, 0.9f));
        var matGround = new LambertainMaterial(texChecker);
        listObj.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, matGround));

        //
        for (int i = -11; i < 11; i++)
        {
            for (int j = -11; j < 11; j++)
            {
                var chooseMat = Random.value;
                var center = new Vector3((float)i + 0.9f * Random.value, 0.2f, (float)j + 0.9f * Random.value);

                if ((center - new Vector3(4f, 0.2f, 0f)).magnitude <= 0.9f)
                {
                    continue;
                }


                if (chooseMat < 0.8f)
                {
                    // diffuse
                    var albedo = Random.ColorHSV();
                    var mat = new LambertainMaterial(albedo);
                    var center1 = center + new Vector3(0f, Random.Range(0f, 0.5f), 0f);
                    listObj.Add(new MovingSphere(center, center1, 0f, 1f, 0.2f, mat));
                }
                else if (chooseMat < 0.95f)
                {
                    // metal
                    var albedo = Random.ColorHSV();
                    float fuzz = Random.Range(0f, 0.5f);
                    var mat = new FuzzyMetalMaterial(albedo, fuzz);
                    listObj.Add(new Sphere(center, 0.2f, mat));
                }
                else
                {
                    // glass
                    var mat = new DielectricMaterial(1.5f);
                    listObj.Add(new Sphere(center, 0.2f, mat));
                }

            }
        }

        //
        var mat1 = new DielectricMaterial(1.5f);
        listObj.Add(new Sphere(new Vector3(0f, 1f, 0f), 1f, mat1));

        var mat2 = new LambertainMaterial(new Color(0.4f, 0.2f, 0.1f));
        listObj.Add(new Sphere(new Vector3(-4f, 1f, 0f), 1.0f, mat2));

        var mat3 = new FuzzyMetalMaterial(new Color(0.7f, 0.6f, 0.5f), 0f);
        listObj.Add(new Sphere(new Vector3(4f, 1f, 0f), 1f, mat3));

        return listObj;
    }

    HittableList TwoSphereScene()
    {
        var listObj = new HittableList();

        var tex = new CheckerTexture(new Color(0.2f, 0.3f, 0.1f), new Color(0.9f, 0.9f, 0.9f));
        var mat = new LambertainMaterial(tex);

        listObj.Add(new Sphere(new Vector3(0f, -10f, 0f), 10f, mat));
        listObj.Add(new Sphere(new Vector3(0f, 10f, 0f), 10f, mat));

        return listObj;
    }

    HittableList TwoPerlinNoiseSphereScene()
    {
        var listObj = new HittableList();

        var mat_0 = new LambertainMaterial(new NoiseTexture(4f));
        listObj.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, mat_0));

        var mat_1 = new LambertainMaterial(new MarbleTexture(4f));
        listObj.Add(new Sphere(new Vector3(0f, 2f, 0f), 2f, mat_1));

        return listObj;
    }

    HittableList EarthScene()
    {
        var listObj = new HittableList();

        var tex = new ImageTexture("earthmap");
        var mat = new LambertainMaterial(tex);

        listObj.Add(new Sphere(new Vector3(0f, 0f, 0f), 2f, mat));

        return listObj;
    }

    HittableList SimpleLightScene()
    {
        var listObj = new HittableList();

        var mat_0 = new LambertainMaterial(new MarbleTexture(4f));
        listObj.Add(new Sphere(new Vector3(0f, -1000f, 0f), 1000f, mat_0));
        listObj.Add(new Sphere(new Vector3(0f, 2f, 0f), 2f, mat_0));

        var mat_1 = new DiffuseLight(new Color(4f, 4f, 4f));
        listObj.Add(new RectangleXY(3f, 5f, 1f, 3f, -2f, mat_1));
        listObj.Add(new Sphere(new Vector3(0f, 7f, 0f), 2f, mat_1));

        return listObj;
    }

    HittableList CornellBoxScene()
    {
        var listObj = new HittableList();

        var matRed = new LambertainMaterial(new Color(0.65f, 0.05f, 0.05f));
        var matWhite = new LambertainMaterial(new Color(0.73f, 0.73f, 0.73f));
        var matGreen = new LambertainMaterial(new Color(0.12f, 0.45f, 0.15f));
        var matLight = new DiffuseLight(new Color(15f, 15f, 15f));

        listObj.Add(new RectangleYZ(0f, 555f, 0f, 555f, 555f, matGreen));
        listObj.Add(new RectangleYZ(0f, 555f, 0f, 555f, 0f, matRed));
        listObj.Add(new RectangleXZ(213f, 343f, 227f, 332f, 554f, matLight));
        listObj.Add(new RectangleXZ(0f, 555f, 0f, 555f, 0f, matWhite));
        listObj.Add(new RectangleXZ(0f, 555f, 0f, 555f, 555f, matWhite));
        listObj.Add(new RectangleXY(0f, 555f, 0f, 555f, 555f, matWhite));

        listObj.Add(new Box(new Vector3(130f, 0f, 65f), new Vector3(295f, 165f, 230f), matWhite));
        listObj.Add(new Box(new Vector3(265f, 0f, 295f), new Vector3(430f, 330f, 460f), matWhite));

        return listObj;
    }





    void Start()
    {
        // camera
        var cam = GetComponent<RayCamera>();
        cam = cam ? cam : new RayCamera();
        cam.Setup(textureWidthHeight.x / textureWidthHeight.y);

        // scene
        HittableList listSceneObj;
        switch (scene)
        {
            case Scene.RandomSphereScene:
            default:
                listSceneObj = RandomSphereScene();
                break;

            case Scene.TwoSphereScene:
                listSceneObj = TwoSphereScene();
                break;

            case Scene.TwoPerlinNoiseSphereScene:
                listSceneObj = TwoPerlinNoiseSphereScene();
                break;

            case Scene.EarthScene:
                listSceneObj = EarthScene();
                break;

            case Scene.SimpleLightScene:
                Debug.Log("SimpleLightScene uses fixed settings");
                backgroundColor = Color.black;
                cam.lookFrom = new Vector3();
                cam.lookAt = new Vector3();
                cam.verticalFov = 20f;
                cam.Setup(textureWidthHeight.x / textureWidthHeight.y);

                listSceneObj = SimpleLightScene();
                break;

            case Scene.CornellBoxScene:
                Debug.Log("CornellBoxScene uses fixed settings");
                backgroundColor = Color.black;
                cam.lookFrom = new Vector3(278f, 278f, -800f);
                cam.lookAt = new Vector3(278f, 278f, 0f);
                cam.verticalFov = 40f;
                textureWidthHeight = new Vector2Int(600, 600);

                cam.Setup(textureWidthHeight.x / textureWidthHeight.y);

                listSceneObj = CornellBoxScene();
                break;
        }


        // image
        int textureWidth = textureWidthHeight.x;
        int textureHeight = textureWidthHeight.y;
        textureResult = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, true, true);


        float startTime = Time.realtimeSinceStartup;

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
                    //pixelColor += RayColor(ray, listSceneObj, maxDepth);
                    pixelColor += RayColor(ray, backgroundColor, listSceneObj, maxDepth);
                }
                WriteColor(textureResult, x, y, pixelColor, samplesPerPixel);
            }
        }

        textureResult.Apply();
        Debug.Log($"Render time: {Time.realtimeSinceStartup - startTime} sec");

        try
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "output_images");
            Directory.CreateDirectory(dir);

            string pngPath = Path.Combine(dir, $"{System.DateTime.Now.Ticks}.png");

            var pngData = textureResult.EncodeToPNG();
            File.WriteAllBytes(pngPath, pngData);

            Debug.Log($"Save to: {pngPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

    }


}
