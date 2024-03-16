using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapView : MonoBehaviour, IPointerClickHandler
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

    private List<MapAirplaneSteerPoint> airplaneSteerPoints;
    public Transform mapAirplaneTargetSymbol;
    private List<GameObject> flightPathPoints;
    private List<MapThreatEvent> threatEventLines;
    private List<MapActiveCircle> mapActiveCircles;

    private FlightPath flightPath;
    public GameObject pointPrefab;

    public GameObject pointDraggedOnMouse;

    private float flightVelocity;
    public GameObject mapThreatOverPathPrefab;
    public GameObject mapActiveCirclePrefab;

    private float milesPerLengthUnit;
    private Vector3 lastClickPosition;

    public GameObject floatingMenuMapViewPrefab;
    private GameObject activeFloatingMenu;

    private const float distToNewSteerPointInEdgesMiles = 5f;

    public void initMapViewByScenario(Scenario scenario, MainContentManager mainContentManager)
    {
        this.scenario = scenario;
        this.mainContentManager = mainContentManager;
        activeFloatingMenu = null;

        flightPathPoints = new List<GameObject>();
        threatEventLines = new List<MapThreatEvent>();

        calculateMilesPerLengthUnit(MAP_BOTTOM_STEER_POINT, MAP_TOP_STEER_POINT, this.transform.GetComponent<RectTransform>().sizeDelta.y);

        flightVelocity = 500f;

        airplaneSteerPoints = new List<MapAirplaneSteerPoint>();
        for (int i=0; i< scenario.airplaneSteerPoints.Length; i++)
        {
            Scenario.SteerPoint loopAirplaneSteerPoint = scenario.airplaneSteerPoints[i];
            GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPointPrefab) as GameObject;
            newMapSteerPoint.transform.parent = this.transform;
            newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(ToXmapPos(loopAirplaneSteerPoint.E), ToYmapPos(loopAirplaneSteerPoint.N));
            newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this, loopAirplaneSteerPoint, i);
            airplaneSteerPoints.Add(newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>());
        }
        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();

        mapActiveCircles = new List<MapActiveCircle>();
        foreach (Scenario.ActiveMapCircle activeMapCircle in scenario.activeMapCircles)
        {
            GameObject newMapActiveCicle = Instantiate(mapActiveCirclePrefab) as GameObject;
            newMapActiveCicle.transform.parent = this.transform;
            newMapActiveCicle.GetComponent<MapActiveCircle>().initMapActiveCircle(this, activeMapCircle);
            mapActiveCircles.Add(newMapActiveCicle.GetComponent<MapActiveCircle>());
        }

    }

    public void addSteerPointBeforeIndex(int steerPointIndex)
    {
        GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPointPrefab) as GameObject;
        newMapSteerPoint.transform.parent = this.transform;

        Vector2 newPosition = Vector2.zero;
        if (steerPointIndex == 0)
            newPosition = airplaneSteerPoints[0].GetComponent<RectTransform>().anchoredPosition + (airplaneSteerPoints[0].GetComponent<RectTransform>().anchoredPosition - airplaneSteerPoints[1].GetComponent<RectTransform>().anchoredPosition).normalized * (distToNewSteerPointInEdgesMiles / milesPerLengthUnit);
        else
            newPosition = (airplaneSteerPoints[steerPointIndex].GetComponent<RectTransform>().anchoredPosition + airplaneSteerPoints[steerPointIndex - 1].GetComponent<RectTransform>().anchoredPosition) / 2f;
        newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = newPosition;
        Scenario.SteerPoint newSteerPoint = new Scenario.SteerPoint(ToStringSTPT(newPosition.y, "N", mapSize), ToStringSTPT(newPosition.x, "E", mapSize));
        newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this, newSteerPoint, steerPointIndex);
        airplaneSteerPoints.Insert(steerPointIndex, newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>());

        for (int i=0; i< airplaneSteerPoints.Count; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().setSteerPointIndex(i);

        Array.Resize(ref scenario.airplaneSteerPoints, airplaneSteerPoints.Count);
        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            scenario.airplaneSteerPoints[i] = airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().getSteerPointObject();

        if (steerPointIndex <= scenario.airPlaneSteerPointOfRelease)
            scenario.airPlaneSteerPointOfRelease++;

        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();
    }

    public void addSteerPointAfterIndex(int steerPointIndex)
    {
        GameObject newMapSteerPoint = Instantiate(mapAirplaneSteerPointPrefab) as GameObject;
        newMapSteerPoint.transform.parent = this.transform;

        Vector2 newPosition = Vector2.zero;
        if (steerPointIndex == airplaneSteerPoints.Count-1)
            newPosition = airplaneSteerPoints[airplaneSteerPoints.Count - 1].GetComponent<RectTransform>().anchoredPosition + (airplaneSteerPoints[airplaneSteerPoints.Count - 1].GetComponent<RectTransform>().anchoredPosition - airplaneSteerPoints[airplaneSteerPoints.Count - 2].GetComponent<RectTransform>().anchoredPosition).normalized * (distToNewSteerPointInEdgesMiles / milesPerLengthUnit);
        else
            newPosition = (airplaneSteerPoints[steerPointIndex].GetComponent<RectTransform>().anchoredPosition + airplaneSteerPoints[steerPointIndex + 1].GetComponent<RectTransform>().anchoredPosition) / 2f;
        newMapSteerPoint.GetComponent<RectTransform>().anchoredPosition = newPosition;
        Scenario.SteerPoint newSteerPoint = new Scenario.SteerPoint(ToStringSTPT(newPosition.y, "N", mapSize), ToStringSTPT(newPosition.x, "E", mapSize));
        newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>().initMapAirplaneSteerPoint(this, newSteerPoint, steerPointIndex);
        airplaneSteerPoints.Insert(steerPointIndex+1, newMapSteerPoint.GetComponent<MapAirplaneSteerPoint>());

        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().setSteerPointIndex(i);

        Array.Resize(ref scenario.airplaneSteerPoints, airplaneSteerPoints.Count);
        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            scenario.airplaneSteerPoints[i] = airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().getSteerPointObject();

        if (steerPointIndex < scenario.airPlaneSteerPointOfRelease)
            scenario.airPlaneSteerPointOfRelease++;

        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();
    }

    public void removeSteerPointInIndex(int steerPointIndex)
    {
        
        Destroy(airplaneSteerPoints[steerPointIndex].gameObject);
        airplaneSteerPoints.RemoveAt(steerPointIndex);
        RemoveAt(ref scenario.airplaneSteerPoints, steerPointIndex);

        for (int i = 0; i < airplaneSteerPoints.Count; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().setSteerPointIndex(i);

        if (steerPointIndex <= scenario.airPlaneSteerPointOfRelease)
            scenario.airPlaneSteerPointOfRelease = Mathf.Max(scenario.airPlaneSteerPointOfRelease - 1, 1);


        airplaneSteerPointDragged();
        updateMapViewAfterSteerPointChanged();

    }

    public void setAsReleaseSteerPoint(int steerPointIndex)
    {
        scenario.airPlaneSteerPointOfRelease = steerPointIndex;
        airplaneSteerPointDragged();
    }

    public void calculateMilesPerLengthUnit(string minStpt, string maxStpt, float mapSize)
    {

        float minContinuesSteerPoint = MapView.ToContinousSteerPoint(minStpt);
        float maxContinuesSteerPoint = MapView.ToContinousSteerPoint(maxStpt);
        milesPerLengthUnit = (maxContinuesSteerPoint - minContinuesSteerPoint) * 60f / mapSize;
    }


    public void airplaneSteerPointDragged()
    {
        for (int i = 0; i < airplaneSteerPoints.Count - 1; i++)
            airplaneSteerPoints[i].GetComponent<MapAirplaneSteerPoint>().makeNavLine(airplaneSteerPoints[i + 1].transform);
        airplaneSteerPoints[airplaneSteerPoints.Count - 1].GetComponent<MapAirplaneSteerPoint>().hideNavLine();
        mapAirplaneTargetSymbol.position = airplaneSteerPoints[scenario.airPlaneSteerPointOfRelease].transform.position;
        mapAirplaneTargetSymbol.transform.eulerAngles = new Vector3(0f, 0f, airplaneSteerPoints[scenario.airPlaneSteerPointOfRelease - 1].GetComponent<MapAirplaneSteerPoint>().getZRotation());
    }

    public void airplaneSteerPointEndDrag()
    {
        updateMapViewAfterSteerPointChanged();
        mainContentManager.setMapViewModified();
    }

    private void updateMapViewAfterSteerPointChanged()
    {
        flightPath = new FlightPath(flightVelocity);
        flightPath.buildNavSectionsByAirplaneNav(airplaneSteerPoints, milesPerLengthUnit);
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

        foreach (MapThreatEvent loopThreatEventLine in threatEventLines)
            Destroy(loopThreatEventLine.gameObject);
        threatEventLines.Clear();
        foreach (Scenario.ActiveThreat activeThreat in scenario.activeThreats)
        {
            Threat threat = mainContentManager.getSimulatorDatabase().getThreatByIndex(activeThreat.activeThreatLinkIndex);
            string threatName = threat.threatName;
            foreach (Scenario.ActiveThreat.ActiveThreatEvent activeThreatEvent in activeThreat.activeThreatEvents)
            {
                //GameObject threatEventGameObject = Instantiate(threatEventPrefab) as GameObject;
                //threatEventGameObject.transform.parent = this.transform;
                //threatEventGameObject.GetComponent<MapThreatEvent>().initMapThreatEvent(this, activeThreatEvent);

                Threat.ThreatLock threatLock = threat.threatLocks[activeThreatEvent.threatLockListIndex];
                string threatLockName = threatLock.threatLockName;
                float activeThreatEventDist = activeThreatEvent.threatEventDistance;
                Vector2 activeThreatEventPos = flightPath.getPosInMilesDist(activeThreatEventDist);
                Vector2 activeThreatEventPointAtPos = flightPath.getPosInMilesDist(activeThreatEventDist + 0.1f);

                //threatEventGameObject.GetComponent<RectTransform>().anchoredPosition = activeThreatEventPos;

                float diffy = activeThreatEventPointAtPos.y - activeThreatEventPos.y;
                float diffx = activeThreatEventPointAtPos.x - activeThreatEventPos.x;
                float rot_z = Mathf.Atan2(diffy, diffx) * Mathf.Rad2Deg;
                //threatEventGameObject.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90f);

                //threatEventGameObject.GetComponent<MapThreatEvent>().setText(threatName + " " + threatLockName);
                //threatEventLines.Add(threatEventGameObject.GetComponent<MapThreatEvent>());

            }

            //GameObject mapThreatOverPath = Instantiate(mapThreatOverPathPrefab) as GameObject;
            //mapThreatOverPath.transform.parent = this.transform;
            //mapThreatOverPath.GetComponent<MapThreatOverPath>().initMesh(this, activeThreat);
        }
    }

    public void threatEventDragged(MapThreatEvent threatEvent, Vector2 position)
    {

        Vector2 anchoredPosition = getMapAnchoredPosition(position);
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

        threatEvent.activeThreatEvent.threatEventDistance = newDist;

    }

    // =================== Map Circles ====================

    public void addNewActiveMapCircle()
    {
        closeFloatingMenu();
        Debug.Log("add new map circle!");
    }

    // =================== General Functions ====================

    public void saveChangesFromMapViewToDatabase()
    {

    }

    public FlightPath getFlightPath()
    {
        return flightPath;
    }

    //public float timeToMilesDist(float time)
    //{
    //    return (flightVelocity / (60f * 60f)) * time;
    //}

    public float getMilesPerLengthUnit()
    {
        return milesPerLengthUnit;
    }

    public Vector2 getMapAnchoredPosition(Vector3 position)
    {
        pointDraggedOnMouse.GetComponent<RectTransform>().position = position;

        return pointDraggedOnMouse.GetComponent<RectTransform>().anchoredPosition;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (activeFloatingMenu != null)
        //    closeAllFloatingMenus();
        //else
        //{
        //    addFloatingMenuMapView(getMapAnchoredPosition(eventData.position));
        //    lastClickPosition = eventData.position;
        //}
        addFloatingMenuMapView(getMapAnchoredPosition(eventData.position));
        lastClickPosition = eventData.position;
    }

    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        Array.Resize(ref arr, arr.Length - 1);
    }

    public void closeAllFloatingMenus()
    {
        closeFloatingMenu();
        foreach (MapAirplaneSteerPoint airplaneSteerPoint in airplaneSteerPoints)
            airplaneSteerPoint.closeFloatingMenu();
        foreach (MapActiveCircle mapActiveCircle in mapActiveCircles)
            mapActiveCircle.closeFloatingMenu();

    }

    private void addFloatingMenuMapView(Vector2 mapPosition)
    {
        closeAllFloatingMenus();
        GameObject floatingMenu = Instantiate(floatingMenuMapViewPrefab) as GameObject;
        floatingMenu.transform.parent = this.transform;
        floatingMenu.GetComponent<RectTransform>().anchoredPosition = mapPosition;
        floatingMenu.GetComponent<FloatingMenuMapView>().initFloatingMapView(this);
        activeFloatingMenu = floatingMenu;
    }

    public void closeFloatingMenu()
    {
        if (activeFloatingMenu != null)
        {
            Destroy(activeFloatingMenu);
            activeFloatingMenu = null;
        }
    }

    public SimulatorDatabase getSimulatorDatabase()
    {
        return mainContentManager.getSimulatorDatabase();
    }




    // ====================== SteerPoint Conversions ==========================

    // ----------------------- From string STPT to map coordiantes ------------

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

    // ----------------------- From map coordiantes to string STPT ------------

    public static string ToStringSTPT(float val, string axe, float mapLength)
    {
        float relativePos = val / mapLength;
        float maxContinousSteerPoint = ToContinousSteerPoint(axe == "N" ? MAP_TOP_STEER_POINT : MAP_RIGHT_STEER_POINT);
        float minContinousSteerPoint = ToContinousSteerPoint(axe == "N" ? MAP_BOTTOM_STEER_POINT : MAP_LEFT_STEER_POINT);
        float continousSteerPoint = minContinousSteerPoint + (maxContinousSteerPoint - minContinousSteerPoint) * relativePos;
        return FromContinousToStringFormatSteerPoint(continousSteerPoint);
    }

    public static string FromContinousToStringFormatSteerPoint(float contSteerPoint)
    {
        float fullPart = Mathf.Floor(contSteerPoint);
        float notFullPart = contSteerPoint - fullPart;
        float minStpt = notFullPart * 60f;
        return Mathf.RoundToInt(fullPart).ToString() + minStpt.ToString("0.000");
    }



}
