
using UnityEngine;



public class PerlinNoise
{
    private const int POINT_COUNT = 256;
    private float[] ranfloat;
    private int[] perm_x, perm_y, perm_z;




    public PerlinNoise()
    {
        ranfloat = new float[POINT_COUNT];
        for (int i = 0; i < POINT_COUNT; ++i)
        {
            ranfloat[i] = Random.value;
        }

        perm_x = PerlinGeneratePerm();
        perm_y = PerlinGeneratePerm();
        perm_z = PerlinGeneratePerm();
    }


    public float Value(Vector3 p)
    {
        int i = ((int)(4f * p.x)) & 255;
        int j = ((int)(4f * p.y)) & 255;
        int k = ((int)(4f * p.z)) & 255;

        return ranfloat[perm_x[i] ^ perm_y[j] ^ perm_z[k]];
    }



    private int[] PerlinGeneratePerm()
    {
        var p = new int[POINT_COUNT];

        for (int i = 0; i < POINT_COUNT; i++)
        {
            p[i] = i;
        }

        // permute
        for (int i = POINT_COUNT - 1; i > 0; i--)
        {
            int target = Random.Range(0, i + 1);
            int tmp = p[i];
            p[i] = p[target];
            p[target] = tmp;
        }

        return p;
    }


}

