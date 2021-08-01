using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Helpers
{
    public static Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point)
    {
        Vector3 startToP = point - start;
        Vector3 startToEnd = end - start;

        float ste2 = startToEnd.x * startToEnd.x + startToEnd.y * startToEnd.y;  //startToEnd.sqrMagnitude;

        float dot = Vector3.Dot(startToP, startToEnd);

        float t = dot / ste2;

        if (t < 0)
        {
            return start;
        }
        else if (t > 1)
        {
            return end;
        }
        else
        {
            return start + (startToEnd * t);
        }
    }

    public static Vector3 GetMousePositionInEditor()
    {
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = Camera.current.pixelHeight - mousePosition.y;
        mousePosition = Camera.current.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0;
        return mousePosition;
    }

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

    public static MassPoint[] GetMassPoints()
    {
        var sBodies = Object.FindObjectsOfType<SoftBodyObject>();
        var array = sBodies.SelectMany( item => item.massPoints).Distinct().ToArray();
        return array;
    }

    public static float Cross2D(Vector2 A, Vector2 B)
    {
        return A.x * B.y - A.y * B.x;
    }
}
