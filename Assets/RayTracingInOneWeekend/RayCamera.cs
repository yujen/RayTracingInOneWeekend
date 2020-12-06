
using UnityEngine;





public class RayCamera : MonoBehaviour
{
    [Header("RayCamera參數")]

    /// <summary>
    /// vertical field-of-view in degrees
    /// </summary>
    [SerializeField]
    public float verticalFov = 70f;

    [SerializeField]
    public Vector3 lookFrom = new Vector3(-2f, 2f, 1f);
    [SerializeField]
    public Vector3 lookAt = Vector3.zero;
    [SerializeField]
    public Vector3 vup = Vector3.up;

    [SerializeField]
    public float aperture = 2f;
    

    [Header("Debug用 執行時會被重新複寫 無法修改")]

    public float viewportWidth;
    public float viewportHeight;
    public Vector3 horizontal;
    public Vector3 vertical;
    public Vector3 lowerLeftCorner;
    public float lensRadius;
    public float focusDistance;

    public Vector3 w;
    public Vector3 u;
    public Vector3 v;



    public void Setup(float aspectRatio)
    {
        float theta = verticalFov * Mathf.Deg2Rad;
        float h = Mathf.Tan(theta / 2f);
        viewportHeight = 2f * h;
        viewportWidth = aspectRatio * viewportHeight;


        w = (lookFrom - lookAt).normalized;
        u = Vector3.Cross(vup, w).normalized;
        v = Vector3.Cross(w, u);


        focusDistance = (lookFrom - lookAt).magnitude;

        horizontal = focusDistance * viewportWidth * u;
        vertical = focusDistance * viewportHeight * v;

        lowerLeftCorner = lookFrom - (horizontal / 2f) - (vertical / 2f) - (focusDistance * w);

        lensRadius = aperture / 2f;
    }



    public Ray GetRay(float s, float t)
    {
        var rd = lensRadius * Random.insideUnitCircle;
        var offset = u * rd.x + v * rd.y;

        return new Ray(lookFrom + offset, lowerLeftCorner + (s * horizontal) + (t * vertical) - lookFrom - offset);
    }


    
}
