using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;



namespace RayTracingInOneWeekendGPU
{
    public class RayTracedSphere : RayTracedObject
    {


        public override int Size()
        {
            // 4bytes * 8 in struct Sphere
            //return (4 * 8);
            return Marshal.SizeOf(typeof(Sphere));
        }

        public Sphere GetData()
        {
            // Careful to handle colorspaces here.
            var albedo = GetComponent<MeshRenderer>().sharedMaterial.GetColor("_BaseColor").linear;
            albedo *= colorMultiplier;

            Sphere sphere;
            sphere.Albedo = new Vector3(albedo.r, albedo.g, albedo.b);
            sphere.Radius = transform.localScale.x / 2f;
            sphere.Center = transform.position;
            sphere.Material = (int)objectMaterial;

            return sphere;
        }


    }

}
