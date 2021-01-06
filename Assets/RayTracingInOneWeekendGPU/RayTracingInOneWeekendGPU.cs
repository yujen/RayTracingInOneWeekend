using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace RayTracingInOneWeekendGPU
{
    public class RayTracingInOneWeekendGPU : MonoBehaviour
    {
        [SerializeField]
        RayTracedSphere[] listSphere;



        List<RayTracedSphere> listSphereInst = new List<RayTracedSphere>();


        private void OnGUI()
        {
            if (GUILayout.RepeatButton("Instantiate Sphere"))
            {
                var sphere = listSphere[Random.Range(0, listSphere.Length)];
                var randomPos = new Vector3(Random.Range(-3f, 3f), Random.Range(0f, 5f), Random.Range(-3f, 3f));
                var inst = GameObject.Instantiate(sphere, randomPos, Quaternion.identity);

                inst.GetComponent<MeshRenderer>().material.color = Random.ColorHSV(0f, 1f, 0f, 1f, 0.5f, 1f);
                listSphereInst.Add(inst);

                if (listSphereInst.Count > 20)
                {
                    GameObject.Destroy(listSphereInst[0].gameObject);
                    listSphereInst.RemoveAt(0);
                }
            }

        }

    }

}
