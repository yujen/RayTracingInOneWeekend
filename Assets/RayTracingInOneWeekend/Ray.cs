using UnityEngine;




public class Ray
{
    public Ray(Vector3 origin, Vector3 direction, float time = 0f)
    {
        this.origin = origin;
        this.direction = direction;
        this.time = time;
    }

    /// <summary>
    /// 射線頂端位置
    /// </summary>
    public Vector3 At(float t)
    {
        return origin + direction * t;
    }


    /// <summary>
    /// 射線起始點
    /// </summary>
    public Vector3 origin;

    /// <summary>
    /// 射線方向, 不一定是單位向量
    /// </summary>
    public Vector3 direction;

    /// <summary>
    /// 
    /// </summary>
    public float time;
}

