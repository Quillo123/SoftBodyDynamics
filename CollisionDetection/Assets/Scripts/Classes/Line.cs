using System;
using UnityEngine;

[Serializable]
public class Line
{
    public Vector2 start = Vector2.zero, end = Vector2.zero;

    public Line() { }

    public Line(Vector2 _start, Vector2 _end)
    {
        start = _start;
        end = _end;
    }

    public static Line Clone(Line newLine)
    {
        return new Line(newLine.start, newLine.end);
    }

    //Function found at https://blog.dakwamine.fr/?p=1943
    public static Vector2 GetIntersection(Line A, Line B, out bool found)
    {
        Vector2 retVal;
        found = LineSegementsIntersect(A.start, A.end, B.start, B.end, out retVal);
        return retVal;
    }

    /// <summary>
    /// Test whether two line segments intersect. If so, calculate the intersection point.
    /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
    /// </summary>
    /// <param name="p">Vector to the start point of p.</param>
    /// <param name="p2">Vector to the end point of p.</param>
    /// <param name="q">Vector to the start point of q.</param>
    /// <param name="q2">Vector to the end point of q.</param>
    /// <param name="intersection">The point of intersection, if any.</param>
    /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
    /// </param>
    /// <returns>True if an intersection point was found.</returns>
    private static bool LineSegementsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2,
        out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
    {
        intersection = new Vector2();

        var r = p2 - p;
        var s = q2 - q;
        var rxs = Cross2D(r, s);
        var qpxr = Cross2D((q - p), r);

        // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
        if (rxs == 0 && qpxr == 0)
        {
            // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
            // then the two lines are overlapping,
            //**if (considerCollinearOverlapAsIntersect)
                //if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                    //return true;**

            // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
            // then the two lines are collinear but disjoint.
            // No need to implement this expression, as it follows from the expression above.
            return false;
        }

        // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
        if (rxs == 0 && qpxr != 0)
            return false;

        // t = (q - p) x s / (r x s)
        var t = Cross2D((q - p), s) / rxs;

        // u = (q - p) x r / (r x s)

        var u = Cross2D((q - p), r) / rxs;

        // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
        // the two line segments meet at the point p + t r = q + u s.
        if (rxs != 0 && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        {
            // We can calculate the intersection point using either t or u.
            intersection = p + t * r;

            // An intersection was found.
            return true;
        }

        // 5. Otherwise, the two line segments are not parallel but do not intersect.
        return false;
    }

    public static float Cross2D(Vector2 A, Vector2 B)
    {
        return A.x * B.y - A.y * B.x;
    }

    public Vector2 Center() { return (start + end) / 2; }

    public Vector2 Normal()
    {
        Vector2 diff = start - end;
        return new Vector2(-diff.y, diff.x);
    }

    public Vector2 ClosestPoint(Vector2 p)
    {
        Vector2 startToP = p - start;
        Vector2 startToEnd = end - start;

        float ste2 = startToEnd.x * startToEnd.x + startToEnd.y * startToEnd.y;  //startToEnd.sqrMagnitude;

        float dot = Vector2.Dot(startToP, startToEnd);

        float t = dot / ste2;

        if(t < 0)
        {
            return start;
        }
        else if(t > 1)
        {
            return end;
        }
        else
        {
            return start + (startToEnd * t);

        }


    }
}











    /*
    float tmp = (B.end.x - B.start.x) * (A.end.y - A.start.y) - (B.end.y - B.start.y) * (A.end.x - A.start.x);

    if (tmp == 0)
    {
        // No solution!
        found = false;
        return Vector2.zero;
    }

    float mu = ((A.start.x - B.start.x) * (A.end.y - A.start.y) - (A.start.y - B.start.y) * (A.end.x - A.start.x)) / tmp;

    found = true;

    Vector2 intersect = new Vector2(
        B.start.x + (B.end.x - B.start.x) * mu,
        B.start.y + (B.end.y - B.start.y) * mu
    );



    float Ax, Ay, Bx, By;
    if (A.start.x > A.end.x)
    {
        Ax = A.start.x;
        Bx = A.end.x;

    }
    else
    {
        Ax = A.end.x;
        Bx = A.start.x;
    }

    if (A.start.y > A.end.x)
    {
        Ay = A.start.y;
        By = A.end.y;
    }
    else
    {
        Ay = A.end.y;
        By = A.start.y;
    }

    if (Bx < intersect.x && intersect.x < Ax)
    {
        if(By < intersect.y && intersect.y < Ay)
        {
            found = true;
        }
        else
        {
            found = false;
        }
    }
    else
    {
        found = false;
    }


    if (found)
    {
        if (B.start.x > B.end.x)
        {
            Ax = B.start.x;
            Bx = B.end.x;

        }
        else
        {
            Ax = B.end.x;
            Bx = B.start.x;
        }

        if (B.start.y > B.end.x)
        {
            Ay = B.start.y;
            By = B.end.y;
        }
        else
        {
            Ay = B.end.y;
            By = B.start.y;
        }

        if (Bx < intersect.x && intersect.x < Ax)
        {
            if (By < intersect.y && intersect.y < Ay)
            {
                found = true;
            }
            else
            {
                found = false;
            }
        }
        else
        {
            found = false;
        }
    }

    if(found == true)
    {
        return intersect;
    }

    return Vector2.zero;
}


    */

