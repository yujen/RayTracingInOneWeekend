using UnityEngine;



namespace RayTracingInOneWeekendGPU
{
    public struct Sphere
    {
        public Vector3 Center;
        public float Radius;
        public int Material;
        public Vector3 Albedo;
    }

    /// <summary>
    /// These values must match the values kMaterialXxx constants in Shading.cginc
    /// </summary>
    public enum ObjectMaterial
    {
        Invalid = 0,
        Lambertian = 1,
        Metal = 2,
        Dielectric = 3,
    }



    public abstract class RayTracedObject : MonoBehaviour
    {

        [SerializeField]
        protected ObjectMaterial objectMaterial;

        [SerializeField, Range(0.01f, 1.5f)]
        protected float colorMultiplier = 1.0f;


        private Matrix4x4 m_lastTransform = Matrix4x4.identity;
        private float m_lastColorMultiplier;
        private Color m_lastColor;


        protected void OnEnable()
        {
            RayTracer.Instance.NotifySceneChanged();
            m_lastTransform = transform.localToWorldMatrix;
        }

        protected void OnDisable()
        {
            if (RayTracer.Instance != null)
            {
                RayTracer.Instance.NotifySceneChanged();
            }
        }

        protected void Update()
        {
            var color = GetComponent<MeshRenderer>().sharedMaterial.GetColor("_BaseColor");

            if (m_lastTransform != transform.localToWorldMatrix
                || m_lastColorMultiplier != colorMultiplier
                || m_lastColor != color)
            {
                m_lastColorMultiplier = colorMultiplier;
                m_lastColor = color;
                m_lastTransform = transform.localToWorldMatrix;
                RayTracer.Instance.NotifySceneChanged();
            }
        }


        /// <summary>
        /// Size of object's data struct in compute shader
        /// </summary>
        /// <returns></returns>
        public abstract int Size();

    }

}
