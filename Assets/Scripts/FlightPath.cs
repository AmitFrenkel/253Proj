using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightPath
{
    public class NavSection
    {
        public Vector2 startPos;
        public Vector2 endPos;
        public float startNavTime;
        public float milesPerLengthUnit;
        public float startDist;
        public float length;


        public virtual void calculateLength()
        {

        }

        public float getLength()
        {
            return length;
        }

        public virtual Vector2 getPosInLocalDist(float dist)
        {
            return Vector2.zero;
        }

        public virtual float getDistOfClosestPointOnPath(Vector2 searchPoint)
        {
            return -1f;
        }
    }

    public class StrightNavSection : NavSection
    {
        public StrightNavSection(float milesPerLengthUnit)
        {
            this.milesPerLengthUnit = milesPerLengthUnit;
        }

        public override void calculateLength()
        {
            length = (endPos - startPos).magnitude * milesPerLengthUnit;
        }

        public override Vector2 getPosInLocalDist(float dist)
        {
            return startPos + (endPos - startPos).normalized * (dist / milesPerLengthUnit);
        }

        public override float getDistOfClosestPointOnPath(Vector2 searchPoint)
        {
            Vector2 u = startPos - searchPoint;
            Vector2 v = endPos - startPos;
            float t = -Vector2.Dot(v, u) / Vector2.Dot(v, v);
            if (t >= 0f & t <= 1f)
            {
                return startDist + t * length;
            }
                
            else
            {
                if (Vector2.SqrMagnitude(startPos - searchPoint) < Vector2.SqrMagnitude(endPos - searchPoint))
                    return startDist;
                else
                    return startDist + length;
            }
        }
    }

    public class TurnNavSection : NavSection
    {
        public Vector2 centerOfTurn;
        public float turnRadius;
        public float turnAngle;

        public TurnNavSection(float milesPerLengthUnit)
        {
            this.milesPerLengthUnit = milesPerLengthUnit;
        }

        public override void calculateLength()
        {
            length = Mathf.Abs(turnAngle) * turnRadius * milesPerLengthUnit;
        }

        public override Vector2 getPosInLocalDist(float dist)
        {
            float angle = (dist / milesPerLengthUnit) / turnRadius;
            return centerOfTurn + rotate((startPos - centerOfTurn), angle * Mathf.Sign(turnAngle * -1f));
        }

        public override float getDistOfClosestPointOnPath(Vector2 searchPoint)
        {
            Vector2 generalClosestPointOfCircle = centerOfTurn + (searchPoint - centerOfTurn).normalized * turnRadius;
            float distToStartPos = Vector2.SqrMagnitude(searchPoint - startPos);
            float distToEndPos = Vector2.SqrMagnitude(searchPoint - endPos);
            float distToClosestPoint = Vector2.SqrMagnitude(searchPoint - generalClosestPointOfCircle);

            float minDist = Mathf.Min(distToStartPos, distToEndPos, distToClosestPoint);
            if (minDist == distToStartPos)
                return startDist;
            else if (minDist == distToEndPos)
                return (startDist + length);
            else
            {
                // calculate angle
                Vector2 centerToStart = (startPos - centerOfTurn);
                Vector2 centerToClosest = (generalClosestPointOfCircle - centerOfTurn);
                float angle = Mathf.Acos(Vector2.Dot(centerToStart, centerToClosest) * Mathf.Sign(turnAngle) /  (turnRadius * turnRadius));
                if (angle > 0f & angle < Mathf.Abs(turnAngle))
                    return startDist + (angle / Mathf.Abs(turnAngle)) * length;
                return startDist;
            }            
        }
    }

    private List<NavSection> navSections;
    private float milesPerLengthUnit;
    private float flightVelocity;

    private const float milesStepForForwardCalculation = 0.1f;


    public FlightPath(float flightVelocity)
    {
        this.flightVelocity = flightVelocity;
    }

    public void buildNavSectionsByAirplaneNav(List<MapAirplaneSteerPoint> airplaneSteerPoints, float milesPerLengthUnit)
    {
        this.milesPerLengthUnit = milesPerLengthUnit;
        navSections = new List<NavSection>();
        float turnRadius = 15f;

        if (airplaneSteerPoints.Count < 2)
        {
            Debug.Log("Airplane nav is less than 2 points, can't build nav sections!");
            return;
        }

        StrightNavSection newStrightNavSection = new StrightNavSection(milesPerLengthUnit);
        newStrightNavSection.startPos = airplaneSteerPoints[0].transform.GetComponent<RectTransform>().anchoredPosition;
        newStrightNavSection.endPos = airplaneSteerPoints[1].transform.GetComponent<RectTransform>().anchoredPosition;
        newStrightNavSection.startDist = 0f;
        newStrightNavSection.calculateLength();
        navSections.Add(newStrightNavSection);

        for (int stpIndx = 1; stpIndx < airplaneSteerPoints.Count - 1; stpIndx++)
        {
            TurnNavSection newTurnNavSection = new TurnNavSection(milesPerLengthUnit);
            newTurnNavSection.startDist = navSections[navSections.Count - 1].startDist + navSections[navSections.Count - 1].getLength();
            newTurnNavSection.startPos = navSections[navSections.Count - 1].endPos;
            Vector2 srcPos = navSections[navSections.Count - 1].startPos;
            Vector2 turnPos = navSections[navSections.Count - 1].endPos;
            Vector2 dstPos = airplaneSteerPoints[stpIndx + 1].transform.GetComponent<RectTransform>().anchoredPosition;
            float turnAngle = getTurnAngleBetweenSteerPoints(srcPos, turnPos, dstPos, turnRadius);
            newTurnNavSection.turnAngle = turnAngle;
            newTurnNavSection.turnRadius = turnRadius;
            Vector2 centerOfTurn = turnPos + rotate((turnPos - srcPos).normalized * turnRadius, (Mathf.PI / 2f) * Mathf.Sign(turnAngle * -1f));
            Vector2 endOfTurn = centerOfTurn + rotate((turnPos - centerOfTurn), turnAngle * -1f);
            newTurnNavSection.centerOfTurn = centerOfTurn;
            newTurnNavSection.endPos = endOfTurn;
            newTurnNavSection.calculateLength();
            navSections.Add(newTurnNavSection);

            newStrightNavSection = new StrightNavSection(milesPerLengthUnit);
            newStrightNavSection.startDist = navSections[navSections.Count - 1].startDist + navSections[navSections.Count - 1].getLength();
            newStrightNavSection.startPos = newTurnNavSection.endPos;
            newStrightNavSection.endPos = dstPos;
            newStrightNavSection.calculateLength();
            navSections.Add(newStrightNavSection);
        }


    }

    public List<NavSection> getNavSectionsOfFlightPath()
    {
        return navSections;
    }

    public float getLengthOfFlightPathInMiles()
    {
        float totalLen = 0f;
        foreach (NavSection navSection in navSections)
            totalLen += navSection.getLength();
        return totalLen;
    }

    public Vector2 getPosInMilesDist(float dist)
    {
        float loopDist = 0f;
        foreach (NavSection navSection in navSections)
        {
            if (dist - loopDist < navSection.getLength())
            {
                return navSection.getPosInLocalDist(dist - loopDist);
            }
            else
            {
                loopDist += navSection.getLength();
            }
        }
        if (dist <= 0f)
            return navSections[0].startPos;
        return navSections[navSections.Count - 1].endPos;
    }

    public Vector2[] getPosAndRightDirInMilesDist(float dist)
    {
        float loopDist = 0f;
        foreach (NavSection navSection in navSections)
        {
            if (dist - loopDist < navSection.getLength())
            {
                Vector2 pos = navSection.getPosInLocalDist(dist - loopDist);
                Vector2 forwardPos = pos;
                if (dist - loopDist + milesStepForForwardCalculation < navSection.length)
                {
                    forwardPos = navSection.getPosInLocalDist(dist - loopDist + milesStepForForwardCalculation);
                }
                else
                {
                    Vector2 backwardPos = navSection.getPosInLocalDist(dist - loopDist - milesStepForForwardCalculation);
                    forwardPos = 2 * pos - backwardPos;
                }
                Vector2 forwardDir = (pos - forwardPos).normalized;
                Vector2 rightVector = rotate(forwardDir, Mathf.PI / 2f);
                return new Vector2[] { pos, rightVector };
            }
            else
            {
                loopDist += navSection.getLength();
            }
        }
        if (dist <= 0f)
        {
            Vector2 pos = navSections[0].getPosInLocalDist(0f);
            Vector2 forwardPos = navSections[0].getPosInLocalDist(milesStepForForwardCalculation);
            Vector2 forwardDir = (pos - forwardPos).normalized;
            Vector2 rightVector = rotate(forwardDir, Mathf.PI / 2f);
            return new Vector2[] { pos, rightVector };
        }
        else
        {
            Vector2 pos = navSections[navSections.Count - 1].endPos;
            Vector2 forwardDir = (navSections[navSections.Count - 1].endPos - navSections[navSections.Count - 1].startPos).normalized;
            Vector2 rightVector = rotate(forwardDir, Mathf.PI / 2f);
            return new Vector2[] { pos, rightVector };
        }
    }

    public float getDistOfClosestPointOnPath(Vector2 searchPoint)
    {
        float distOnPath = Mathf.Infinity;
        float minDistSearchToPath = Mathf.Infinity;

        foreach (NavSection navSection in navSections)
        {
            float distOnPathForNavSection = navSection.getDistOfClosestPointOnPath(searchPoint);
            float minDistSearchToPathForNavSection = Vector2.SqrMagnitude(getPosInMilesDist(distOnPathForNavSection) - searchPoint);
            if (minDistSearchToPathForNavSection < minDistSearchToPath)
            {
                minDistSearchToPath = minDistSearchToPathForNavSection;
                distOnPath = distOnPathForNavSection;
            }
        }
        return distOnPath;
    }

    // =================== Support functions ===================

    public static float getTurnAngleBetweenSteerPoints(Vector2 originalSrcP, Vector2 originalTurnP, Vector2 originalDestP, float turnRaduis)
    {
        // Algorithm based on the solution of example #15 in link: https://www.siyavula.com/read/za/mathematics/grade-12/analytical-geometry/07-analytical-geometry-03

        // Rotate the points such as turn origin is in origin
        // Calculate only for right turns. For left turns, flip the x coordinate.
        Vector2 turnP = new Vector2(-turnRaduis, 0f);
        float alignAlpha = Mathf.Atan2(originalTurnP.x - originalSrcP.x, originalTurnP.y - originalSrcP.y);
        Vector2 originalTurnToDstP = originalDestP - originalTurnP;
        //Vector2 rotatedTurnToDstP = new Vector2(originalTurnToDstP.x * Mathf.Cos(alignAlpha) - originalTurnToDstP.y * Mathf.Sin(alignAlpha), originalTurnToDstP.x * Mathf.Sin(alignAlpha) + originalTurnToDstP.y * Mathf.Cos(alignAlpha));
        Vector2 rotatedTurnToDstP = rotate(originalTurnToDstP, alignAlpha);
        bool isCalculationReflectedToRightTurn = (rotatedTurnToDstP.x < 0f);
        if (isCalculationReflectedToRightTurn)
            rotatedTurnToDstP = new Vector2(-rotatedTurnToDstP.x, rotatedTurnToDstP.y);
        Vector2 dstP = turnP + rotatedTurnToDstP;

        // Find y of tangent
        float pre = 2f * dstP.y * Mathf.Pow(turnRaduis, 2f);
        float dominator = Vector2.SqrMagnitude(dstP);
        float in_sqrt = Mathf.Pow(pre, 2f) - 4f * dominator * (Mathf.Pow(turnRaduis, 4f) - Mathf.Pow(dstP.x, 2f) * Mathf.Pow(turnRaduis, 2f));
        if (in_sqrt < 0)
            Debug.Log("in_sqrt < 0!! why?");
        float sqrt = Mathf.Sqrt(in_sqrt);
        float y1 = (pre + sqrt) / (2f * dominator);
        float y2 = (pre - sqrt) / (2f * dominator);

        // Select the positive y
        float y = 0;
        if (y1 < 0 & y2 > 0)
            y = y2;
        else if (y1 > 0 & y2 < 0)
            y = y1;
        //else
        //{
        //    Debug.Log("no positive and negative solutions!! why?" + ", y1 = " + y1 + ", y2 = " + y2);
        //}

        // Find x
        float xSign = (rotatedTurnToDstP.y > turnRaduis) ? -1f : 1f;
        float x = Mathf.Sqrt(Mathf.Pow(turnRaduis, 2f) - Mathf.Pow(y, 2f)) * xSign;
        float angle = Mathf.PI - Mathf.Atan2(y, x);

        return angle * (isCalculationReflectedToRightTurn ? -1f : 1f);
    }

    public static Vector2 rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }
}
