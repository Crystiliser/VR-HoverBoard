using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSplineDrawn
{
    Stack<Vector3> finalPoints = new Stack<Vector3>();

    //line or loop
    public bool isLooping = false;
     
    public Vector3[] makePath(Vector3[] points)
    {
        finalPoints.Clear();
        if (points.Length >= 4)
        {
            for (int i = 0; i < points.Length; i++)
            {
                //so we dont do looping
                if ((i == 0 || i == points.Length - 2 || i == points.Length - 1) && !isLooping)
                {
                    continue;
                }

                DisplayCatmullRomSpline(i, points);
            }
            return finalPoints.ToArray();
        }
        else
        {
            return null;
        }
    }

    void DisplayCatmullRomSpline(int pos, Vector3[] points)
    {
        //The 4 points we need to form a spline between p1 and p2
        Vector3 p0 = points[ClampListPos(pos - 1, points)];
        Vector3 p1 = points[pos];
        Vector3 p2 = points[ClampListPos(pos + 1, points)];
        Vector3 p3 = points[ClampListPos(pos + 2, points)];
        

        //The spline's resolution
        //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work
        float resolution = 0.2f;

        //How many times should we loop?
        int loops = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= loops; i++)
        {
            //Which t position are we at?
            float t = i * resolution;

            //Find the coordinate between the end points with a Catmull-Rom spline
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

            //Draw this line segment
            finalPoints.Push(newPos);
            
        }
    }

    int ClampListPos(int pos, Vector3[] points)
    {
        if (pos < 0)
        {
            pos = points.Length - 1;
        }

        if (pos > points.Length)
        {
            pos = 1;
        }
        else if (pos > points.Length - 1)
        {
            pos = 0;
        }

        return pos;
    }

    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}
