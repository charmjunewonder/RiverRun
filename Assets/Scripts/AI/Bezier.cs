using UnityEngine;
using System.Collections;

public class Bezier : MonoBehaviour{

    Vector3 p0;
    Vector3 p1;
    Vector3 p2;
    Vector3 p3;

    Vector3 b0 = Vector3.zero;
    Vector3 b1 = Vector3.zero;
    Vector3 b2 = Vector3.zero;
    Vector3 b3 = Vector3.zero;

    float Ax;
    float Ay;
    float Az;
    float Bx;
    float By;
    float Bz;
    float Cx;
    float Cy;
    float Cz;

    public Bezier(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3){
        p0 = v0;
        p1 = v1;
        p2 = v2;
        p3 = v3;
    }


    public Vector3 getPosition(float t){
        check();
        float t2 = t * t;
        float t3 = t2 * t;
        float x = Ax * t3 + Bx * t2 + Cx * t + p0.x;
        float y = Ay * t3 + By * t2 + Cy * t + p0.y;
        float z = Az * t3 + Bz * t2 + Cz * t + p0.z;
        return new Vector3(x, y, z);
    }

    private void check()
    {
        if (p0 != b0 || p1 != b1 || p2 != b2 || p3 != b3)
        {
            set();
            b0 = p0;
            b1 = p1;
            b2 = p2;
            b3 = p3;
        }
    }

    private void set()
    {
        Cx = 3 * ((p0.x + p1.x) - p0.x);
        Bx = 3 * ((p3.x + p2.x) - (p0.x + p1.x)) - Cx;
        Ax = p3.x - p0.x - Cx - Bx;

        Cy = 3 * ((p0.y + p1.y) - p0.y);
        By = 3 * ((p3.y + p2.y) - (p0.y + p1.y)) - Cy;
        Ay = p3.y - p0.y - Cy - By;

        Cz = 3 * ((p0.z + p1.z) - p0.z);
        Bz = 3 * ((p3.z + p2.z) - (p0.z + p1.z)) - Cz;
        Az = p3.z - p0.z - Cz - Bz;
    }

    public Vector3 getDestination() { return p3; }
}