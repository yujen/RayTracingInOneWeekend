
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
    CornellSmokeBoxScene,
    FinalScene
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

    [SerializeField, Range(1, 10000)]
    private int samplesPerPixel = 8;

    [SerializeField, Range(1, 100)]
    private int maxDepth = 8;

    [SerializeField]
    private Texture2D textureResult;






    Color RayColor(Ray ray, Color background, Hittable world, int depth)
    {
        // If we've exceeded the ray bounce limit, no more light is gathered
        if (depth <= 0)
        {
            return Color.black;
        }

        // If the ray hits nothing, return the background color.
        HitRecord rec = null;
        if (world.IsHit(ray, 0.001f, float.PositiveInfinity, ref rec) == false)
        {
            return background;
        }


        Color albedo;
        Ray scattered;
        float pdf;  // probability density function
        Color emitted = rec.objMaterial.Emitted(rec);

        if (rec.objMaterial.Scatter(ray, rec, out albedo, out scattered, out pdf) == false)
        {
            return emitted;
        }


        var cosinePDF = new CosinePDF(rec.normal);
        scattered = new Ray(rec.p, cosinePDF.Generate(), ray.time);
        pdf = cosinePDF.Value(scattered.direction);


        /*
        // =======================
        var on_light = new Vector3(Utils.RandomRange(213, 343), 554, Utils.RandomRange(227, 332));
        var to_light = on_light - rec.p;
        var distance_squared = to_light.sqrMagnitude;
        to_light.Normalize();

        if (Vector3.Dot(to_light, rec.normal) < 0f)
        {
            return emitted;
        }


        float light_area = (343 - 213) * (332 - 227);
        float light_cosine = Mathf.Abs(to_light.y);
        if (light_cosine < 0.000001f)
        {
            return emitted;
        }

        pdf = distance_squared / (light_cosine * light_area);
        scattered = new Ray(rec.p, to_light, ray.time);
        // =======================
        */

        //
        return emitted
            + (albedo
            * rec.objMaterial.ScatteringPDF(ray, rec, scattered)
            * RayColor(scattered, background, world, depth - 1) / pdf);

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

        // material
        var matRed = new LambertainMaterial(new Color(0.65f, 0.05f, 0.05f));
        var matWhite = new LambertainMaterial(new Color(0.73f, 0.73f, 0.73f));
        var matGreen = new LambertainMaterial(new Color(0.12f, 0.45f, 0.15f));
        var matLight = new DiffuseLight(new Color(15f, 15f, 15f));

        // 
        listObj.Add(new RectangleYZ(0f, 555f, 0f, 555f, 555f, matGreen));
        listObj.Add(new RectangleYZ(0f, 555f, 0f, 555f, 0f, matRed));
        listObj.Add(new FlipFace(new RectangleXZ(213f, 343f, 227f, 332f, 554f, matLight)));
        listObj.Add(new RectangleXZ(0f, 555f, 0f, 555f, 0f, matWhite));
        listObj.Add(new RectangleXZ(0f, 555f, 0f, 555f, 555f, matWhite));
        listObj.Add(new RectangleXY(0f, 555f, 0f, 555f, 555f, matWhite));

        //
        Hittable box_0 = new Box(Vector3.zero, new Vector3(165f, 330f, 165f), matWhite);
        box_0 = new RotateY(box_0, 15f);
        box_0 = new Translate(box_0, new Vector3(265f, 0f, 295f));
        listObj.Add(box_0);

        Hittable box_1 = new Box(Vector3.zero, new Vector3(165f, 165f, 165f), matWhite);
        box_1 = new RotateY(box_1, -18f);
        box_1 = new Translate(box_1, new Vector3(130f, 0f, 65f));
        listObj.Add(box_1);


        return listObj;
    }

    HittableList CornellSmokeBoxScene()
    {
        var listObj = new HittableList();

        // material
        var matRed = new LambertainMaterial(new Color(0.65f, 0.05f, 0.05f));
        var matWhite = new LambertainMaterial(new Color(0.73f, 0.73f, 0.73f));
        var matGreen = new LambertainMaterial(new Color(0.12f, 0.45f, 0.15f));
        var matLight = new DiffuseLight(new Color(15f, 15f, 15f));

        // 
        listObj.Add(new RectangleYZ(0f, 555f, 0f, 555f, 555f, matGreen));
        listObj.Add(new RectangleYZ(0f, 555f, 0f, 555f, 0f, matRed));
        listObj.Add(new RectangleXZ(113f, 443f, 127f, 432f, 554f, matLight));
        listObj.Add(new RectangleXZ(0f, 555f, 0f, 555f, 0f, matWhite));
        listObj.Add(new RectangleXZ(0f, 555f, 0f, 555f, 555f, matWhite));
        listObj.Add(new RectangleXY(0f, 555f, 0f, 555f, 555f, matWhite));

        //
        Hittable box_0 = new Box(Vector3.zero, new Vector3(165f, 330f, 165f), matWhite);
        box_0 = new RotateY(box_0, 15f);
        box_0 = new Translate(box_0, new Vector3(265f, 0f, 295f));
        box_0 = new ConstantMedium(box_0, 0.01f, Color.black);
        listObj.Add(box_0);

        Hittable box_1 = new Box(Vector3.zero, new Vector3(165f, 165f, 165f), matWhite);
        box_1 = new RotateY(box_1, -18f);
        box_1 = new Translate(box_1, new Vector3(130f, 0f, 65f));
        box_1 = new ConstantMedium(box_1, 0.01f, Color.white);
        listObj.Add(box_1);


        return listObj;
    }

    HittableList FinalScene()
    {
        var listObj = new HittableList();

        // ground
        var listGround = new HittableList();
        var matGround = new LambertainMaterial(new Color(0.48f, 0.83f, 0.53f));
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                float w = 100f;
                float x0 = -1000f + i * w;
                float z0 = -1000f + j * w;
                float y0 = 0f;
                float x1 = x0 + w;
                float y1 = Random.Range(1f, 101f);
                float z1 = z0 + w;

                listGround.Add(new Box(new Vector3(x0, y0, z0), new Vector3(x1, y1, z1), matGround));
            }
        }
        listObj.Add(new BVHNode(listGround, 0f, 1f));

        // light
        var matLight = new DiffuseLight(new Color(7f, 7f, 7f));
        listObj.Add(new RectangleXZ(123f, 423f, 147f, 412f, 554f, matLight));

        // moving sphere
        var center1 = new Vector3(400f, 400f, 200f);
        var center2 = center1 + new Vector3(30f, 0f, 0f);
        var matMovingSphere = new LambertainMaterial(new Color(0.7f, 0.3f, 0.1f));
        listObj.Add(new MovingSphere(center1, center2, 0f, 1f, 50f, matMovingSphere));

        // dielectric sphere
        listObj.Add(new Sphere(new Vector3(260f, 150f, 45f), 50f, new DielectricMaterial(1.5f)));

        // metal sphere
        var matMetal = new FuzzyMetalMaterial(new Color(0.8f, 0.8f, 0.9f), 1f);
        listObj.Add(new Sphere(new Vector3(0f, 150f, 145f), 50f, matMetal));

        // blue sphere
        var blueSphere = new Sphere(new Vector3(360, 150, 145), 70f, new DielectricMaterial(1.5f));
        listObj.Add(blueSphere);
        listObj.Add(new ConstantMedium(blueSphere, 0.2f, new Color(0.2f, 0.4f, 0.9f)));

        // boundary, fog
        var fog = new Sphere(Vector3.zero, 5000f, new DielectricMaterial(1.5f));
        listObj.Add(new ConstantMedium(fog, 0.0001f, Color.white));


        // earth sphere
        var matEarth = new LambertainMaterial(new ImageTexture("earthmap"));
        listObj.Add(new Sphere(new Vector3(400, 200, 400), 100, matEarth));

        // noise sphere
        var matNoise = new LambertainMaterial(new NoiseTexture(0.1f));
        listObj.Add(new Sphere(new Vector3(220, 280, 300), 80, matNoise));

        // box sphere
        var listBoxSphere = new HittableList();
        var matBoxSphere = new LambertainMaterial(new Color(0.73f, 0.73f, 0.73f));
        for (int i = 0; i < 1000; i++)
        {
            var center = new Vector3(Random.Range(0, 165), Random.Range(0, 165), Random.Range(0, 165));
            listBoxSphere.Add(new Sphere(center, 10f, matBoxSphere));
        }
        var bvhBoxSphere = new BVHNode(listBoxSphere, 0f, 1f);
        var rotateBoxSphere = new RotateY(bvhBoxSphere, 15f);
        var translateBoxSphere = new Translate(rotateBoxSphere, new Vector3(-100f, 270f, 395f));
        listObj.Add(translateBoxSphere);


        //
        return listObj;
    }


    void Start()
    {
        float startTime = Time.realtimeSinceStartup;


        // camera
        var cam = GetComponent<RayCamera>();
        cam = cam ? cam : new RayCamera();
        cam.Setup(textureWidthHeight);

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
                cam.Setup(textureWidthHeight);

                listSceneObj = SimpleLightScene();
                break;

            case Scene.CornellBoxScene:
                Debug.Log("CornellBoxScene uses fixed settings");
                backgroundColor = Color.black;
                cam.lookFrom = new Vector3(278f, 278f, -800f);
                cam.lookAt = new Vector3(278f, 278f, 0f);
                cam.vup = Vector3.up;
                cam.verticalFov = 40f;
                cam.focusDistance = 10f;
                cam.aperture = 0f;
                cam.shutterTime = new Vector2(0f, 1f);
                //textureWidthHeight = new Vector2Int(600, 600);
                cam.Setup(textureWidthHeight);

                listSceneObj = CornellBoxScene();
                break;

            case Scene.CornellSmokeBoxScene:
                Debug.Log("CornellSmokeBoxScene uses fixed settings");
                backgroundColor = Color.black;
                cam.lookFrom = new Vector3(278f, 278f, -800f);
                cam.lookAt = new Vector3(278f, 278f, 0f);
                cam.vup = Vector3.up;
                cam.verticalFov = 40f;
                cam.focusDistance = 10f;
                cam.aperture = 0f;
                cam.shutterTime = new Vector2(0f, 1f);
                //textureWidthHeight = new Vector2Int(600, 600);
                cam.Setup(textureWidthHeight);

                listSceneObj = CornellSmokeBoxScene();
                break;

            case Scene.FinalScene:
                Debug.Log("FinalScene uses fixed settings");
                backgroundColor = Color.black;
                cam.lookFrom = new Vector3(478f, 278f, -600f);
                cam.lookAt = new Vector3(278f, 278f, 0f);
                cam.verticalFov = 40f;
                textureWidthHeight = new Vector2Int(800, 800);
                cam.Setup(textureWidthHeight);

                listSceneObj = FinalScene();
                break;
        }


        // image
        int textureWidth = textureWidthHeight.x;
        int textureHeight = textureWidthHeight.y;
        textureResult = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, true, true);

        //
        Debug.Log($"Setup scene time: {Time.realtimeSinceStartup - startTime} sec");
        startTime = Time.realtimeSinceStartup;

        // render
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Color pixelColor = new Color(0f, 0f, 0f, 0f);

                for (int i = 0; i < samplesPerPixel; i++)
                {
                    float u = ((float)x + Utils.RandomNum) / (textureWidth - 1);
                    float v = ((float)y + Utils.RandomNum) / (textureHeight - 1);

                    var ray = cam.GetRay(u, v);
                    pixelColor += RayColor(ray, backgroundColor, listSceneObj, maxDepth);
                }
                WriteColor(textureResult, x, y, pixelColor, samplesPerPixel);
            }
        }

        Debug.Log($"Render time: {Time.realtimeSinceStartup - startTime} sec");


        SaveTexture(textureResult);

    }

    /// <summary>
    /// https://answers.unity.com/questions/1340371/endcodetopng-resulting-in-dark-image.html
    /// </summary>
    private void SaveTexture(Texture2D texture)
    {
        texture.Apply();

        var rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

        Graphics.Blit(texture, rt);

        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, true, true);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        try
        {
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "output_images");
            Directory.CreateDirectory(dir);

            string pngPath = Path.Combine(dir, $"{System.DateTime.Now.Ticks}.png");

            var pngData = tex.EncodeToPNG();
            File.WriteAllBytes(pngPath, pngData);

            Debug.Log($"Save to: {pngPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        Object.Destroy(tex);

    }



}
