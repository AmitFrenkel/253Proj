using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapView : MonoBehaviour
{

    public const string MAP_TOP_STEER_POINT = "3430.000";
    public const string MAP_BOTTOM_STEER_POINT = "3300.000";
    public const string MAP_LEFT_STEER_POINT = "3500.000";
    public const string MAP_RIGHT_STEER_POINT = "3630.000";
    public const float mapSize = 800f;

    private MainContentManager mainContentManager;
    private Scenario scenario;

    public GameObject mapAirplaneSteerPointPrefab;
    public GameObject threatEventPrefab;

    private List<GameObject> airplaneSteerPoints;
    public Transform mapAirplaneTargetSymbol;
    private List<GameObject> flightPathPoints;
    private List<GameObject> threatEventLines;

    private FlightPath flightPath;
    public GameObject pointPrefab;

    public GameObject pointDraggedOnMouse;

    private float flightVelocity;

    public void initMapViewByScenario(Scenario scenario, MainContentManager mainContentManager)
    {
        this.scenario = scenario;
        this.mainContentManager = mainContentManager;
        flightPathPoints = new List<GameObject>();
        threatEventLines = new List<GameObject>();

        flightVelocity = 500f;

        airplaneSteerPoints = new List<GameObject>();
        foreach (Scenario.SteerPoint loopAirplaneSteerPoint in scenario.airplaneSteerPoints)
        {
            GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPointPrefab) as GameObject;
            newMapSteerPoint.transform.parent = this.transform;
            newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(ToXmapPos(loopAirplaneSteerPoint.E), ToYmapPos(loopAirplaneSteerPoint.N));
            newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this);
            airplaneSteerPoints.Add(newMapSteerPoint);
        }
        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();




    }

    public void airplaneSteerPointDragged()
    {
        for (int i = 0; i < airplaneSteerPoints.Count - 1; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().makeALine(airplaneSteerPoints[i + 1].transform);
        mapAirplaneTargetSymbol.position = airplaneSteerPoints[1].transform.position;
        mapAirplaneTargetSymbol.transform.eulerAngles = new Vector3(0f, 0f, airplaneSteerPoints[0].GetComponent<MapAirplaneSteerPoint>().getZRotation());
    }

    public void airplaneSteerPointEndDrag()
    {
        updateMapViewAfterSteerPointChanged();
        mainContentManager.setMapViewModified();
    }

    private void updateMapViewAfterSteerPointChanged()
    {
        flightPath = new FlightPath(flightVelocity);
        flightPath.setMilesPerLengthUnit(MAP_BOTTOM_STEER_POINT, MAP_TOP_STEER_POINT, this.transform.GetComponent<RectTransform>().sizeDelta.y);
        flightPath.buildNavSectionsByAirplaneNav(airplaneSteerPoints);

        foreach (GameObject loopFlightPathPoint in flightPathPoints)
            Destroy(loopFlightPathPoint);
        flightPathPoints.Clear();
        float spacing = 0.5f;
        float loopDist = 0f;
        float totalLength = flightPath.getLengthOfFlightPathInMiles();
        while (loopDist < totalLength)
        {
            GameObject newPoint = Instantiate(pointPrefab) as GameObject;
            newPoint.transform.parent = this.transform;
            newPoint.GetComponent<RectTransform>().anchoredPosition = flightPath.getPosInMilesDist(loopDist);
            flightPathPoints.Add(newPoint);
            loopDist += spacing;
        }

        foreach (GameObject loopThreatEventLine in threatEventLines)
            Destroy(loopThreatEventLine);
        threatEventLines.Clear();
        foreach (Scenario.ActiveThreat activeThreat in scenario.activeThreats)
        {
            Threat threat = mainContentManager.getSimulatorDatabase().getThreatByIndex(activeThreat.activeThreatLinkIndex);
            string threatName = threat.threatName;
            foreach (Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent in activeThreat.activeThreatEvents)
            {
                GameObject threatEventGameObject = Instantiate(threatEventPrefab) as GameObject;
                threatEventGameObject.transform.parent = this.transform;
                threatEventGameObject.GetComponent<MapThreatEvent>().initMapThreatEvent(this, activeThreatEvent);

                Threat.ThreatLock threatLock = threat.threatLocks[activeThreatEvent.threatLockListIndex];
                string threatLockName = threatLock.threatLockName;
                float activeThreatEventTime = activeThreatEvent.threatEventTime;
                float activeThreatEventDist = (flightVelocity / (60f * 60f)) * activeThreatEventTime;
                Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(activeThreatEventDist);
                Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(activeThreatEventDist + 0.1f);

                threatEventGameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

                float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
                float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
                float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
                threatEventGameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

                threatEventGameObject.GetComponent<MapThreatEvent>().setText(threatName + " " + threatLockName);
                threatEventLines.Add(threatEventGameObject);

            }
        }
    }

    public void threatEventDragged(MapThreatEvent threatEvent, Vector2 position)
    {
        pointDraggedOnMouse.GetComponent<RectTransform>().position = position;
        Vector2 anchoredPosition = pointDraggedOnMouse.GetComponent<RectTransform>().anchoredPosition;

        float newDist = flightPath.getDistOfClosestPointOnPath(anchoredPosition);

        Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(newDist);
        Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(newDist + 0.1f);
        if (activeThreatEventPos == activeThreatEventPointAtPos)
            activeThreatEventPointAtPos = 2f * activeThreatEventPos - flightPath.getPosInMilesDist(newDist - 0.1f);

        threatEvent.gameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

        float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
        float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
        float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
        threatEvent.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

        float newTime = newDist / (flightVelocity / (60f * 60f));
        threatEvent.activeThreatEvent.threatEventTime = newTime;

    }

    public void saveChangesFromMapViewToDatabase()
    {

    }

    // ====================== SteerPoint Conversions ==========================

    public static float ToContinousSteerPoint(string stp)
    {
        float deg = float.Parse(stp.Substring(0, 2));
        float min = float.Parse(stp.Substring(2, 2)) + (float.Parse(stp.Substring(5, 3))/1000f);
        return deg + min/60f;
    }

    public static float SteerPointToMapPosition(string stpPoint, string stpMax, string stpMin, float mapLength)
    {
        return ((ToContinousSteerPoint(stpPoint) - ToContinousSteerPoint(stpMin))/ (ToContinousSteerPoint(stpMax) - ToContinousSteerPoint(stpMin))) * mapLength;
    }

    public static float ToXmapPos(string stpPointEast)
    {
        return SteerPointToMapPosition(stpPointEast, MAP_RIGHT_STEER_POINT, MAP_LEFT_STEER_POINT, mapSize);
    }

    public static float ToYmapPos(string stpPointNorth)
    {
        return SteerPointToMapPosition(stpPointNorth, MAP_TOP_STEER_POINT, MAP_BOTTOM_STEER_POINT, mapSize);
    }
}
