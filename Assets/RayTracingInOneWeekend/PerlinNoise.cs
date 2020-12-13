
using UnityEngine;



public class PerlinNoise
{
    private const int POINT_COUNT = 256;

    private float[] randFloat;
    private Vector3[] randVector;

    private int[] perm_x, perm_y, perm_z;




    public PerlinNoise()
    {
        randFloat = new float[POINT_COUNT];
        for (int i = 0; i < POINT_COUNT; i++)
        {
            randFloat[i] = Random.value;
        }

        randVector = new Vector3[POINT_COUNT];
        for (int i = 0; i < POINT_COUNT; i++)
        {
            randVector[i] = Random.insideUnitSphere;
        }


        perm_x = PerlinGeneratePerm();
        perm_y = PerlinGeneratePerm();
        perm_z = PerlinGeneratePerm();
    }


    /// <summary>
    /// 
    /// </summary>
    public float Value(Vector3 p)
    {
        int i = ((int)(p.x)) & 255;
        int j = ((int)(p.y)) & 255;
        int k = ((int)(p.z)) & 255;

        return randFloat[perm_x[i] ^ perm_y[j] ^ perm_z[k]];
    }

    /// <summary>
    /// Trilinearly interpolated, smoothed
    /// </summary>
    public float TrilinearInterpValue(Vector3 p)
    {
        float u = p.x - Mathf.Floor(p.x);
        float v = p.y - Mathf.Floor(p.y);
        float w = p.z - Mathf.Floor(p.z);

        // Hermitian Smoothing
        u = u * u * (3 - 2 * u);
        v = v * v * (3 - 2 * v);
        w = w * w * (3 - 2 * w);

        int i = Mathf.FloorToInt(p.x);
        int j = Mathf.FloorToInt(p.y);
        int k = Mathf.FloorToInt(p.z);

        var block = new float[2, 2, 2];

        for (int di = 0; di < 2; di++)
        {
            for (int dj = 0; dj < 2; dj++)
            {
                for (int dk = 0; dk < 2; dk++)
                {
                    block[di, dj, dk] = randFloat[perm_x[(i + di) & 255] ^ perm_y[(j + dj) & 255] ^ perm_z[(k + dk) & 255]];
                }
            }
        }

        // trilinear interp
        float accum = 0f;
        for (int a = 0; a < 2; a++)
        {
            for (int b = 0; b < 2; b++)
            {
                for (int c = 0; c < 2; c++)
                {
                    accum += (a * u + (1 - a) * (1 - u)) *
                            (b * v + (1 - b) * (1 - v)) *
                            (c * w + (1 - c) * (1 - w)) * block[a, b, c];
                }
            }
        }


        return accum;
    }

    /// <summary>
    /// Random vectors on the lattice points
    /// </summary>
    public float PerlinInterpValue(Vector3 p)
    {
        float u = p.x - Mathf.Floor(p.x);
        float v = p.y - Mathf.Floor(p.y);
        float w = p.z - Mathf.Floor(p.z);
        int i = Mathf.FloorToInt(p.x);
        int j = Mathf.FloorToInt(p.y);
        int k = Mathf.FloorToInt(p.z);

        var block = new Vector3[2, 2, 2];


        for (int di = 0; di < 2; di++)
        {
            for (int dj = 0; dj < 2; dj++)
            {
                for (int dk = 0; dk < 2; dk++)
                {
                    block[di, dj, dk] = randVector[perm_x[(i + di) & 255] ^ perm_y[(j + dj) & 255] ^ perm_z[(k + dk) & 255]];
                }
            }
        }

        // perlin interp
        float uu = u * u * (3 - 2 * u);
        float vv = v * v * (3 - 2 * v);
        float ww = w * w * (3 - 2 * w);
        float accum = 0f;

        for (int a = 0; a < 2; a++)
        {
            for (int b = 0; b < 2; b++)
            {
                for (int c = 0; c < 2; c++)
                {
                    var weight_v = new Vector3(u - a, v - b, w - c);

                    accum += (a * uu + (1 - a) * (1f - uu))
                            * (b * vv + (1 - b) * (1f - vv))
                            * (c * ww + (1 - c) * (1f - ww))
                            * Vector3.Dot(block[a, b, c], weight_v);
                }
            }
        }

        return accum;
    }

    public float TurbulenceValue(Vector3 p, int depth = 7)
    {
        Vector3 temp_p = p;
        float weight = 1f;
        float accum = 0f;

        for (int i = 0; i < depth; i++)
        {
            accum += weight * PerlinInterpValue(temp_p);
            weight *= 0.5f;
            temp_p *= 2f;
        }

        return Mathf.Abs(accum);
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

