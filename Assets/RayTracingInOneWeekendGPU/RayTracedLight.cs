using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



namespace RayTracingInOneWeekendGPU
{
    [RequireComponent(typeof(Light))]
    public class RayTracedLight : RayTracedSphere
    {
        private new Light light;



        protected override void OnEnable()
        {
            light = GetComponent<Light>();
            m_meshRenderer = GetComponent<MeshRenderer>();
            objectMaterial = ObjectMaterial.DiffuseLight;


            var color = m_meshRenderer.sharedMaterial.GetColor("_BaseColor");
            light.color = color;

            Update();
            RayTracer.Instance.NotifySceneChanged();
        }




    }

}
